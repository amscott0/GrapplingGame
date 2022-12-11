using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class UIDisplay : MonoBehaviour
{
    Slider _dodgeSlider;
    public static Func<int> observables;

    private void Start(){
        _dodgeSlider = GetComponent<Slider>();
        _dodgeSlider.value = 1;
    }
    private void Update(){
        if(observables?.Invoke() == 1){
            _dodgeSlider.value = 0;
        }
        else{
            if(_dodgeSlider.value < 1.0f)

            _dodgeSlider.value += 1.75f * Time.deltaTime;
        }
    }
}
