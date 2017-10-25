using PluginInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using PluginInterfaces.TableSpecs;
using Newtonsoft.Json;
using PluginInterfaces.Forms;

namespace GeneratorPlugins.GuangMingRegularQuarterlyReports
{
    public struct ShiftingTable
    {
        public string vendor;
        public string region;
        public string description;
        public decimal[,] shiftingMatrix;
        public decimal[,] summaryMatrix;
        public static string[] summaryLabel = { "品牌转换", "原有消费者购买增加/减少",
            "购买清单中增加/删除品牌", "新增/流失品类消费者" };
    }

    [Export(typeof(IPlugin))]
    [ExportMetadata("Name", "光明得失报告生成")]
    [ExportMetadata("Description", "生成光明各个地区、品牌的得失报告PPT，并插入最终PPT的对应位置")]
    [ExportMetadata("Period", "季度")]
    public class Plugin : IPlugin
    {
        [Import]
        IPVAutomation pv { get; set; }

        private List<SortedDictionary<string, List<int>>> SelectBrands(Field brandField, ICollection<int> brandIndices)
        {
            var fields = brandField.items;
            //classify indices by parent
            var parentDict = new Dictionary<FieldItem, List<int>>();
            foreach (var idx in brandIndices)
            {
                var parent = fields.ElementAt(idx).parent;
                if (!parentDict.ContainsKey(parent))
                {
                    parentDict.Add(parent, new List<int>());
                }
                parentDict[parent].Add(idx);
            }

            /*
             * a
             * b
             * c
             * - c1
             * - c2
             * a selected -> c1 and c2 should be selected
             * c selected -> a, b, c should be selected
             * they are different
             */
            var UnionSet = new List<List<int>>();
            foreach (var p in parentDict)
            {
                var init = new List<int>(p.Value);
                foreach (var child in p.Value)
                {
                    if (fields.ElementAt(child).children.Count != 0)
                    {
                        init.Remove(child);
                        UnionSet.Add(new List<int>() { child });
                    }
                }
                if (init.Count != 0)
                {
                    UnionSet.Add(init);
                }
            }

            Func<FieldItem, IEnumerable<int>> SelectLowestChildren = null;
            SelectLowestChildren = (parent) =>
            {
                List<int> res = new List<int>();
                if (parent.children.Count == 0)
                {
                    if (parent.index != -1)
                    {
                        res.Add(parent.index);
                    }
                }
                else
                {
                    foreach (var child in parent.children)
                    {
                        res.AddRange(SelectLowestChildren(child));
                    }
                }
                return res;
            };

            Func<FieldItem, IEnumerable<int>, List<int>> SelectLowestChildrenExcept = (parent, excludes) =>
            {
                var res = from child in parent.children
                          where !excludes.Contains(child.index)
                          select SelectLowestChildren(child);
                var l = new List<int>();
                foreach (var t in res)
                {
                    l.AddRange(t);
                }
                return l;
            };


            List<SortedDictionary<string, List<int>>> result = new List<SortedDictionary<string, List<int>>>();
            foreach (var k in UnionSet)
            {
                var currParent = fields.ElementAt(k[0]).parent;
                var currChildren = k;
                List<int> others = new List<int>();
                while (currParent != null)
                {
                    others.AddRange(SelectLowestChildrenExcept(currParent, currChildren));
                    currChildren = new List<int> { currParent.index };
                    currParent = currParent.parent;
                }
                result.Add(new SortedDictionary<string, List<int>>
                {
                    { "comparisons", new List<int>(others.Concat(k).OrderBy(x => x).ToList()) },
                    { "brands", new List<int>(k) },
                });
            }
            return result;
        }
        public string GetNeededInfoDescription()
        {
            List<FormItemBase> forms = new List<FormItemBase>
            {
                new FieldItems
                {
                    name = "disrict",
                    description = "要跑得失的地区" ,
                },
                new FieldItems
                {
                    name = "brands",
                    description = "做分析的品牌",
                },
                new FileForm
                {
                    name = "output",
                    description = "输出文件",
                    filter = "Excel文件(*.xlsx)|*.xlsx",
                    isSave = true,
                }
            };
            return JsonConvert.SerializeObject(forms);
        }

        Field GetFieldByIndex(int index)
        {
            var list = pv.GetFieldList();
            foreach (var f in list)
            {
                if (f.index == index)
                {
                    return pv.FillFieldItemList(f);
                }
            }
            return null;
        }

        public string GetTablesToExecute(string neededInfoJson)
        {
            var info = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(neededInfoJson);
            // Partition the brands
            var brandsField = (Int64)info["brands"]["field"];
            var brandsList = ((Newtonsoft.Json.Linq.JArray)info["brands"]["fieldItems"]).ToObject<List<int>>() ;

            var b = SelectBrands(GetFieldByIndex((int)brandsField), brandsList);

            

            // Construct Spec
            SwitchSpec spec = new SwitchSpec
            {
                brands = 
                {
                    name = "abc",
                    fieldItems = {1,2,3,4,5 }
                },
                period1Field =
                {
                    name = null,
                    fieldItems = {3,4,5,6 }
                }
            };
            return JsonConvert.SerializeObject(b);
        }
    }
}
