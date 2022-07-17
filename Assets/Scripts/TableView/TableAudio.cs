using UnityEngine;

namespace TableView {
    [RequireComponent(typeof(AudioSource))]
    public class TableAudio : MonoBehaviour {
        [SerializeField] private AudioClip _popSound;

        private AudioSource _source;

        private void Awake() => _source = GetComponent<AudioSource>();

        public void PlayPop() {
            _source.pitch = Random.Range(0.5f, 1f);
            _source.PlayOneShot(_popSound);
        }
    }
}
