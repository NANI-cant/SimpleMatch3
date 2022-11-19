using UnityEngine;

namespace Infrastructure{
    public class AdService{
        public void ShowAd(){
            Application.ExternalCall("ShowAd");
        }
    }
}
