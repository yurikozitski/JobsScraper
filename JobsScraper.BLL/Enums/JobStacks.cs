using JobsScraper.BLL.EnumDisplayAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobsScraper.BLL.Enums
{
    public enum JobStacks
    {
        [DjinniParam("JavaScript")]
        JavaScriptFrontEnd = 1,

        [DjinniParam("Fullstack")]
        Fullstack,

        [DjinniParam("Java")]
        Java,

        [DjinniParam(".NET")]
        CSharpDotNET,

        Python,
        PHP,
        NodeJs,
        iOS,
        Android,
        ReactNative,
        CCppEmbeddedSystem,
        Flutter,
        Golang,
        Ruby,
        Scala,
        Salesforce,
        Rust,
        ERPSystems,
        QAManual,
        QAAutomation,
        DesignUIUX,
        TwoDThreeDArtistIllustrator,
        GamedevUnity,
        ProjectManager,
        ProductManager,
        ProductOwner,
        DeliveryManager,
        ScrumMasterAgileCoach,
        ArchitectCTO,
        DevOps,
        Security,
        Sysadmin,
        BusinessAnalyst,
        DataScience,
        DataAnalyst,
        DataEngineer,
        SQLDBA,
        TechnicalWriting,
        Marketing,
        Sales,
        LeadGeneration,
        SEO,
        HR,
        Recruiter,
        CustomerTechnicalSupport,
        HeadChief,
        Finances,
    }
}
