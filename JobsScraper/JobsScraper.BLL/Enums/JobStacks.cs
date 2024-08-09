using JobsScraper.BLL.EnumDisplayAttributes;

namespace JobsScraper.BLL.Enums
{
    public enum JobStacks
    {
        [DjinniParam("JavaScript")]
        [DouParam("Front End")]
        [RobotaUaParam("Front End")]
        [RecruitikaParam("front-end")]
        JavaScriptFrontEnd = 1,

        [DjinniParam("Fullstack")]
        [DouParam("Full Stack")]
        [RobotaUaParam("Full-Stack")]
        [RecruitikaParam("full-stack")]
        Fullstack,

        [DjinniParam("Java")]
        [DouParam("Java")]
        [RobotaUaParam("Java")]
        [RecruitikaParam("java-developer")]
        Java,

        [DjinniParam(".NET")]
        [DouParam(".NET")]
        [RobotaUaParam(".NET")]
        [RecruitikaParam("net")]
        CSharpDotNET,

        [DjinniParam("Python")]
        [DouParam("Python")]
        [RobotaUaParam("Python")]
        [RecruitikaParam("python")]
        Python,

        //PHP,
        //NodeJs,
        //iOS,
        //Android,
        //ReactNative,
        //CCppEmbeddedSystem,
        //Flutter,
        //Golang,
        //Ruby,
        //Scala,
        //Salesforce,
        //Rust,
        //ERPSystems,
        //QAManual,
        //QAAutomation,
        //DesignUIUX,
        //TwoDThreeDArtistIllustrator,
        //GamedevUnity,
        //ProjectManager,
        //ProductManager,
        //ProductOwner,
        //DeliveryManager,
        //ScrumMasterAgileCoach,
        //ArchitectCTO,
        //DevOps,
        //Security,
        //Sysadmin,
        //BusinessAnalyst,
        //DataScience,
        //DataAnalyst,
        //DataEngineer,
        //SQLDBA,
        //TechnicalWriting,
        //Marketing,
        //Sales,
        //LeadGeneration,
        //SEO,
        //HR,
        //Recruiter,
        //CustomerTechnicalSupport,
        //HeadChief,
        //Finances,
    }
}
