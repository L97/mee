using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sybase.DataWindow;

namespace DataWindowDemo_frm
{
    public partial class DataWindowExt : DataWindowControl
    {
        public DataWindowExt()
        {
            InitializeComponent();
        }

        public DataWindowExt(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
