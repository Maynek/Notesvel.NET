//********************************
// (c) 2026 Ada Maynek
// This software is released under the MIT License.
//********************************
namespace Maynek.Notesvel
{
    public enum WorkTargetType
    {
        None,
        NextSite,
        OfficeWord,
        ServiceNarou,
        ServiceKakuyomu,
        ServiceAlphaPolis,
    }

    public class Work
    {
        public static readonly string WORK_DEFAULT_VALUE = "DEFAULT";

        public bool Enabled { get; set; } = true;
        public WorkTargetType Target { get; set; } = WorkTargetType.None;
        public string OutputFileName { get; set; } = WORK_DEFAULT_VALUE;
        public string TemplateFileName { get; set; } = WORK_DEFAULT_VALUE;
    }
}
