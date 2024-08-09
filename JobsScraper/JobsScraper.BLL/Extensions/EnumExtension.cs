using System.Reflection;
using JobsScraper.BLL.EnumDisplayAttributes;
using JobsScraper.BLL.Enums;

namespace JobsScraper.BLL.Extensions
{
    public static class EnumExtension
    {
        public static string ToQueryParam(this Enum en, JobBoards jobBoard)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                Type jobBoardTypeAttribute = null!;

                if (jobBoard is JobBoards.Djinni)
                {
                    jobBoardTypeAttribute = typeof(DjinniParamAttribute);
                }

                if (jobBoard is JobBoards.Dou)
                {
                    jobBoardTypeAttribute = typeof(DouParamAttribute);
                }

                if (jobBoard is JobBoards.RobotaUa)
                {
                    jobBoardTypeAttribute = typeof(RobotaUaParamAttribute);
                }

                if (jobBoard is JobBoards.Recruitika)
                {
                    jobBoardTypeAttribute = typeof(RecruitikaParamAttribute);
                }

                object[] attrs = memInfo[0].GetCustomAttributes(jobBoardTypeAttribute, false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((ParamAttribute)attrs[0]).Text;
                }
            }

            return en.ToString();
        }
    }
}
