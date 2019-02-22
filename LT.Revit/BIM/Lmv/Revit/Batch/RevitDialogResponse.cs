namespace BIM.Lmv.Revit.Batch
{
    using System;
    using System.Collections.Generic;

    internal class RevitDialogResponse
    {
        private readonly Dictionary<string, int> _Overrides;
        public readonly Dictionary<RevitDialogType, string> StandardDialogs;

        public RevitDialogResponse()
        {
            Dictionary<RevitDialogType, string> dictionary = new Dictionary<RevitDialogType, string> {
                { 
                    RevitDialogType.AuditWarning,
                    "TaskDialog_Audit_Warning"
                },
                { 
                    RevitDialogType.FileNameInUse,
                    "TaskDialog_File_Name_In_Use"
                },
                { 
                    RevitDialogType.SaveFile,
                    "TaskDialog_Save_File"
                },
                { 
                    RevitDialogType.ChangesNotSynchronized,
                    "TaskDialog_Local_Changes_Not_Synchronized_With_Central"
                },
                { 
                    RevitDialogType.ChangesNotSaved,
                    "TaskDialog_Changes_Not_Saved"
                },
                { 
                    RevitDialogType.CloseWithoutSaving,
                    "TaskDialog_Close_Project_Without_Saving"
                },
                { 
                    RevitDialogType.DefaultFamilyTemplateInvalid,
                    "TaskDialog_Default_Family_Template_File_Invalid"
                },
                { 
                    RevitDialogType.LostOnImport,
                    "TaskDialog_Elements_Lost_On_Import"
                },
                { 
                    RevitDialogType.UnresolvedReferences,
                    "TaskDialog_Unresolved_References"
                },
                { 
                    RevitDialogType.TaskDialog_Command_Failure_For_Extenal_Command,
                    "TaskDialog_Command_Failure_For_Extenal_Command"
                },
                { 
                    RevitDialogType.TaskDialog_Model_Opened_By_Another_User,
                    "TaskDialog_Model_Opened_By_Another_User"
                },
                { 
                    RevitDialogType.TaskDialog_Rendering_Library_Not_Installed,
                    "TaskDialog_Rendering_Library_Not_Installed"
                }
            };
            this.StandardDialogs = dictionary;
            this._Overrides = new Dictionary<string, int>();
        }

        public void AddOverride(RevitDialogType dialogType, int resultOverride)
        {
            this._Overrides.Add(this.StandardDialogs[dialogType], resultOverride);
        }

        public void ClearOverrides()
        {
            this._Overrides.Clear();
        }

        public int GetOverride(RevitDialogType dialogType) => 
            this.GetOverride(this.StandardDialogs[dialogType]);

        public int GetOverride(string dialogId)
        {
            if (!this.HasOverride(dialogId))
            {
                throw new ArgumentException("No Task Dialog matching Dialog Id \"" + dialogId + "\" was found");
            }
            return this._Overrides[dialogId];
        }

        public bool HasOverride(RevitDialogType dialogType) => 
            this.HasOverride(this.StandardDialogs[dialogType]);

        public bool HasOverride(string dialogId) => 
            this._Overrides.ContainsKey(dialogId);
    }
}

