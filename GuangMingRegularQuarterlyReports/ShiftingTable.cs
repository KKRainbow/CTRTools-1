using Newtonsoft.Json.Linq;
using PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class Plugin : IPlugin
    {
        private Dictionary<List<int>, List<int>> SelectBrands(Field brandField,ICollection<int> brandIndices)
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


            Dictionary<List<int>, List<int>> result = new Dictionary<List<int>, List<int>>();
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
                result.Add(others.Concat(k).OrderBy(x => x).ToList(), k);
            }
            return result;
        }
        public string GetNeededInfoDescription()
        {
            JArray form = new JArray
            {
                new JObject {
                    { "type", (int)InfoType.Field },
                    {"description", "要跑得失的地区" },
                    {"name", "district" }
                },

                new JObject {
                    { "type", (int)InfoType.FieldItems },
                    {"description", "做分析的品牌" },
                    {"name", "brands" }
                },

                new JObject {
                    { "type", (int)InfoType.File },
                    {"description", "输出文件" },
                    {"filter", "*.xslx" },
                    {"name", "output" }
                },
            };
            return form.ToString();
        }
    }
}
