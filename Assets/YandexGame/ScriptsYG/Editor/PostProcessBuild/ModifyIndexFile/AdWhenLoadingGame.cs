
namespace YG.Insides.BuildModify
{
	public static partial class ModifyIndexFile
	{
#if UNITY_WEBGL
        static void SetAdWhenLoadGameValue(ref string fileText)
        {
            InfoYG infoYG = ConfigYG.GetInfoYG();

            if (infoYG.showFirstAd == false)
            {
                fileText = fileText.Replace("let firstAd = true;", "let firstAd = false;");
            }
        }
#endif
	}
}
