using System.Runtime.InteropServices;

namespace Infrastructure{
    public class AdService{
        [DllImport("__Internal")]
        private static extern void ShowYandexAd();
        
        public void ShowAd(){
#if UNITY_EDITOR
#else
        ShowYandexAd();
#endif
        }
    }
}
