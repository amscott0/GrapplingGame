using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

//This class implements the Observer pattern.
public class UIDisplay : MonoBehaviour
{
    Slider _dodgeSlider;
    [SerializeField] public TMP_Text _text;

    private int index = 0;
    public static Func<int> observables; //This variable is public and static, so any class anywhere can subscribe and send a message to the UI

    private void Start(){
        _dodgeSlider = GetComponent<Slider>();
        
        _dodgeSlider.value = 1;
    }
    private void Update(){
        int? value = observables?.Invoke();
        if(value == 1){
            _dodgeSlider.value = 0;
        }
        else if(value == -1){
            index = (index +1)%3;
            if(index == 0) _text.text = "Grapple: AntiGrav";
            if(index == 1) _text.text = "Grapple: Impulse";
            if(index == 2) _text.text = "Grapple: Unequipped";
        }
        else{
            if(_dodgeSlider.value < 1.0f)

            _dodgeSlider.value += 1.75f * Time.deltaTime;
        }
    }
}
