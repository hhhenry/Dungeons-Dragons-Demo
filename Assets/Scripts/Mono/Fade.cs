using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
 
public class Fade : MonoBehaviour {
 
    public CanvasGroup canvasGroup;
	void Start () {
	    canvasGroup.DOFade(1,2).SetLoops(10, LoopType.Yoyo);
	}
    
}