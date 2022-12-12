using UnityEngine;
using System;
using UnityEngine.InputSystem;
//This class and its subclasses are part of the Command pattern, these classes are the commands that can be created
public abstract class Command
{
    public void Execute(){
        //do nothing
    }
}

public class Move : Command
{
    private InputValue val;
    public Move(InputValue val){
        this.val = val;
    }
    new public Vector2 Execute(){
        return this.val.Get<Vector2>();
    }
}
public class Look : Command
{
    private InputValue val;
    public Look(InputValue val){
        this.val = val;
    }
    new public Vector2 Execute(){
        return this.val.Get<Vector2>();
    }
}
public class Jump : Command
{
    private InputValue val;
    public Jump(InputValue val){
        this.val = val;
    }
    new public bool Execute(){
        return this.val.isPressed;
    }
}
public class Sprint : Command
{
    private InputValue val;
    public Sprint(InputValue val){
        this.val = val;
    }
    new public bool Execute(){
        return this.val.isPressed;
    }
}
public class Dodge : Command
{
    private InputValue val;
    public Dodge(InputValue val){
        this.val = val;
    }
    new public bool Execute(){
        return this.val.isPressed;
    }
}
public class Slide : Command
{
    private InputValue val;
    public Slide(InputValue val){
        this.val = val;
    }
    new public bool Execute(){
        return this.val.isPressed;
    }
}
public class Grapple : Command
{
    private InputValue val;
    public Grapple(InputValue val){
        this.val = val;
    }
    new public bool Execute(){
        return this.val.isPressed;
    }
}
public class SwitchGrapple : Command
{
    private InputValue val;
    public SwitchGrapple(InputValue val){
        this.val = val;
    }
    new public bool Execute(){
        return this.val.isPressed;
    }
}