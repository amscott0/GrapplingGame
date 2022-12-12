using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    using UnityEngine.InputSystem;
#endif
//This class and its subclasses implement the Command pattern.
public abstract class GrapplingBehavior
{
    public bool prevClicked = false;
    public bool noPointFound = true;
    public StarterAssetsInputs _input;
    public LineRenderer _lr;

    public Vector3 grapplingPos;

    public Transform _camera, _playerPos;

    
    // public GrapplingBehavior(StarterAssetsInputs inputs, LineRenderer lr, Transform plyr,Transform cmra){
    //     _input = inputs;
    //     _lr = lr;
    //     _camera = cmra;
    //     _playerPos = plyr;
    // }
    
    public abstract Vector3 grapple();
    

}

public class AntiGrav : GrapplingBehavior{

    public AntiGrav(StarterAssetsInputs inputs, LineRenderer lr, Transform plyr,Transform cmra){
        _input = inputs;
        _lr = lr;
        _camera = cmra;
        _playerPos = plyr;
    }
    public override Vector3 grapple(){
        if(_input.grapple){
            
            if(!prevClicked){
                prevClicked = true;
                RaycastHit hit;
                if(Physics.Raycast(_camera.position, _camera.forward, out hit, 100f)){
                    grapplingPos = hit.point;
                    noPointFound = true;
                }
                else{
                    noPointFound = false;
                }
            }
            if(!noPointFound){
                _lr.SetPosition(0, _playerPos.position);
                _lr.SetPosition(1, _playerPos.position);
                return Vector3.zero;
            }
            
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, grapplingPos);
            Debug.Log("grappled to " + grapplingPos);

            float magnitudeMult = MathF.Sqrt(Vector3.Distance(_playerPos.position, grapplingPos));

            return (-magnitudeMult/20) * Vector3.Normalize(_playerPos.position - grapplingPos);
        }
        else{
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, _playerPos.position);

            prevClicked = false;
            // grapplingPos = _playerPos.position;

            return Vector3.zero;
        }
    }
}

public class Impulse : GrapplingBehavior{

    public Impulse(StarterAssetsInputs inputs, LineRenderer lr, Transform plyr,Transform cmra){
        _input = inputs;
        _lr = lr;
        _camera = cmra;
        _playerPos = plyr;
    }
    public override Vector3 grapple(){
        if(_input.grapple){
            
            if(!prevClicked){
                prevClicked = true;
                RaycastHit hit;
                if(Physics.Raycast(_camera.position, _camera.forward, out hit, 100f)){
                    grapplingPos = hit.point;
                    // noPointFound = true;
                    
                    _lr.SetPosition(0, _playerPos.position);
                    _lr.SetPosition(1, grapplingPos);
                    Debug.Log("grappled to " + grapplingPos);

                    float magnitudeMult = MathF.Sqrt(Vector3.Distance(_playerPos.position, grapplingPos));

                    return (-magnitudeMult/0.1f) * Vector3.Normalize(_playerPos.position - grapplingPos);

                }
                else{
                    // noPointFound = false;
                }
            }
            // if(!noPointFound){
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, _playerPos.position);
            return Vector3.zero;
            // }
            
            
        }
        else{
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, _playerPos.position);

            prevClicked = false;
            // grapplingPos = _playerPos.position;

            return Vector3.zero;
        }
    }
}

public class Unequipped : GrapplingBehavior{

    public Unequipped(StarterAssetsInputs inputs, LineRenderer lr, Transform plyr,Transform cmra){
        _input = inputs;
        _lr = lr;
        _camera = cmra;
        _playerPos = plyr;
    }
    public override Vector3 grapple(){
        _lr.SetPosition(0, _playerPos.position);
        _lr.SetPosition(1, _playerPos.position);
        return Vector3.zero;
        
    }
}



/*

while(clicking grappling button){
    wherever hooked, constantly add velocity in that direction




}
*/