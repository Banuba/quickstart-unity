using UnityEngine;
namespace BNB
{
    /// <summary>
    /// This class realised trigger on tarantula
    /// </summary>
    public class TarantulaTrigger : TriggerObject
    {
        protected override void OnMouthOpen()
        {
            gameObject.GetComponentInChildren<Animator>().SetTrigger("IsWalking");
        }
    }
}