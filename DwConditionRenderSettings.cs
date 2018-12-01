using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataWindowDemo_frm
{
    class DwConditionRenderSettings
    {
        public int RenderMode { get; set; }
        public string DatasourceSql { get; set; }
        public List<Items> items { get; set; }
    }
    public class Items
    {
        public string ItemText { get; set; }
        public string MainDatawindowObject { get; set; }
        public string PrintDatawindowObject { get; set; }
        public string ConditionValue { get; set; }
        public string AdditionalButtonSettings { get; set; }
        public string ColumnReflectSettings { get; set; }
        public string ColumnUpdateSettings { get; set; }
        public string NursingRecordSyncScript { get; set; }
        public string SplitRowsWithOnPrinting { get; set; }
        public string NursingNoteHideGroupColumns { get; set; }
        public string NursingNoteFocusColumnAfterInsert { get;set;}
        public string NursingNoteAutoHeightWith { get; set; }
        public string NursingNoteDatawindowZoom { get; set; }    //int  第二组是空值
        public int NursingNotePrintDatawindowZoom { get; set; }  
        public int WordSize { get; set; }  
        public bool IsCaSign { get; set; }  
        public bool EachKey { get; set; } 
        public bool CAKey { get; set; }    
        public string CaSignType { get; set; }
        public string PicturePath { get; set; }
        public string NursingFrequency { get; set; }
        public string DefNursingFrequency { get; set; }
        public string InsertRowMode { get; set; }
        public bool IsAddInOutSummaryManually { get; set; }   
        public string GeneralPopupEditableColumns { get; set; }
        public string GeneralPopupUnitEditableColumns { get; set; }
        public string InOutSummaryTimeRange { get;set; }
        public string InStaticSql { get; set; }
        public string OutStaticSql { get; set; }
        public string InOutSumAccountSql { get; set; }
        public string InSumMapping { get; set; }
        public string OutSumMapping { get; set; }
        public string SpecialInOutNameMapping { get; set; }
        public string InDeptTime { get; set; }
    }
}


