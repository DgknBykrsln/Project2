using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StateMachine<T, TState> where T : MonoBehaviour where TState : struct, Enum
{
    public T Owner { get; private set; }
    public TState CurrentState { get; private set; }

    private readonly Dictionary<TState, MethodInfo> exitMethods = new();
    private readonly Dictionary<TState, MethodInfo> enterMethods = new();
    private readonly Dictionary<TState, MethodInfo> executeMethods = new();
    private readonly Dictionary<TState, MethodInfo> fixedExecuteMethods = new();

    public void Initialize(T owner)
    {
        Owner = owner;
        CacheStateMethods();
    }

    private void CacheStateMethods()
    {
        var ownerType = Owner.GetType();
        var baseType = typeof(T);

        foreach (TState state in Enum.GetValues(typeof(TState)))
        {
            var stateName = state.ToString();

            exitMethods[state] = GetMethod(ownerType, baseType, stateName + "Exit");
            enterMethods[state] = GetMethod(ownerType, baseType, stateName + "Enter");
            executeMethods[state] = GetMethod(ownerType, baseType, stateName + "Execute");
            fixedExecuteMethods[state] = GetMethod(ownerType, baseType, stateName + "FixedExecute");
        }
    }

    private static MethodInfo GetMethod(Type ownerType, Type baseType, string methodName)
    {
        return ownerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
               ?? baseType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    }

    public void ChangeState(TState newState)
    {
        if (exitMethods.TryGetValue(CurrentState, out var exitMethod) && exitMethod != null)
        {
            exitMethod.Invoke(Owner, null);
        }

        CurrentState = newState;

        if (enterMethods.TryGetValue(CurrentState, out var enterMethod) && enterMethod != null)
        {
            enterMethod.Invoke(Owner, null);
        }
    }

    public void Execute()
    {
        if (executeMethods.TryGetValue(CurrentState, out var executeMethod) && executeMethod != null)
        {
            executeMethod.Invoke(Owner, null);
        }
    }

    public void FixedExecute()
    {
        if (fixedExecuteMethods.TryGetValue(CurrentState, out var fixedExecuteMethod) && fixedExecuteMethod != null)
        {
            fixedExecuteMethod.Invoke(Owner, null);
        }
    }
}