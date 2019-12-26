﻿using System;
using System.Linq;
using System.Xml.Serialization;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using taskt.UI.Forms;
using taskt.UI.CustomControls;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("DataTable Commands")]
    [Attributes.ClassAttributes.Description("This command allows you to add a datarow to a DataTable")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to add a datarow to a DataTable.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    public class AddDataRowCommand : ScriptCommand
    {
        [XmlAttribute]
        [Attributes.PropertyAttributes.PropertyDescription("Please indicate the DataTable Name")]
        [Attributes.PropertyAttributes.InputSpecification("Enter a existing DataTable to add rows to.")]
        [Attributes.PropertyAttributes.SampleUsage("**myData**")]
        [Attributes.PropertyAttributes.Remarks("")]
        public string v_DataTableName { get; set; }
        [XmlAttribute]
        [Attributes.PropertyAttributes.PropertyDescription("Please input the data you want entered.")]
        [Attributes.PropertyAttributes.PropertyUIHelper(Attributes.PropertyAttributes.PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [Attributes.PropertyAttributes.InputSpecification("Enter a string of comma seperated values.")]
        [Attributes.PropertyAttributes.SampleUsage("name1,name2,name3,name4")]
        [Attributes.PropertyAttributes.Remarks("")]
        public string v_InputData { get; set; }

        public AddDataRowCommand()
        {
            this.CommandName = "AddDataRowCommand";
            this.SelectionName = "Add DataRow to Datatable";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Core.Automation.Engine.AutomationEngineInstance)sender;
            var dataSetVariable = LookupVariable(engine);
            var vInputData = v_InputData.ConvertToUserVariable(engine);
            DataTable Dt = (DataTable)dataSetVariable.VariableValue;
            vInputData = vInputData.Trim('\\', '"', '/');
            var splittext = vInputData.Split(',');

            int i = 0;
            foreach (string str in splittext)
            {
                splittext[i] = str.Trim('\\', '"', '[', ']');
                if (splittext[i] == null)
                    splittext[i] = string.Empty;
                i++;
            }

            Dt.Rows.Add(splittext.ToArray());
            dataSetVariable.VariableValue = Dt;
        }
        private Script.ScriptVariable LookupVariable(Core.Automation.Engine.AutomationEngineInstance sendingInstance)
        {
            //search for the variable
            var requiredVariable = sendingInstance.VariableList.Where(var => var.VariableName == v_DataTableName).FirstOrDefault();

            //if variable was not found but it starts with variable naming pattern
            if ((requiredVariable == null) && (v_DataTableName.StartsWith(sendingInstance.engineSettings.VariableStartMarker)) && (v_DataTableName.EndsWith(sendingInstance.engineSettings.VariableEndMarker)))
            {
                //reformat and attempt
                var reformattedVariable = v_DataTableName.Replace(sendingInstance.engineSettings.VariableStartMarker, "").Replace(sendingInstance.engineSettings.VariableEndMarker, "");
                requiredVariable = sendingInstance.VariableList.Where(var => var.VariableName == reformattedVariable).FirstOrDefault();
            }

            return requiredVariable;
        }
        public override List<Control> Render(frmCommandEditor editor)
        {
            base.Render(editor);

            RenderedControls.AddRange(CommandControls.CreateDefaultInputGroupFor("v_DataTableName", this, editor));
            RenderedControls.AddRange(CommandControls.CreateDefaultInputGroupFor("v_InputData", this, editor));


            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + " [Add Datarow: " + v_InputData + " to DataTable: " + v_DataTableName + " ]";
        }
    }
}