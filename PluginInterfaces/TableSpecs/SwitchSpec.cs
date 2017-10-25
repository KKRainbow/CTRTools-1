using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterfaces.TableSpecs
{
    public class SwitchSpec
    {
        public FieldDetail demographicField = new FieldDetail();
        public FieldDetail period1Field = new FieldDetail();
        public FieldDetail period2Field = new FieldDetail();
        public FieldDetail dataFilter = new FieldDetail();
        public FieldDetail brands = new FieldDetail();
        public FieldDetail primaryVolume = new FieldDetail();
        public FieldDetail secondaryVolume = new FieldDetail();

        public bool isRolling = false;
        public int periodLength, waveInterval;

        bool summaryMatrix, basicMatrix, shiftingMatrix, controlMatrix, gainLossMatrix;
    }
}
