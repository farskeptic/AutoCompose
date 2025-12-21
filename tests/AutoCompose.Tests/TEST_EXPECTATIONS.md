# Test Expectations Guide

This document explains what each snapshot test should generate and why. Use this to determine if a `.received.cs` file should be accepted as `.verified.cs`.

## Core Principle

**AutoCompose only generates code for interface members that are NOT properly implemented.**

A member is considered "not implemented" if:
- ❌ It doesn't exist at all
- ❌ It exists but has wrong signature (parameters, return type, access modifier)
- ❌ It exists but doesn't satisfy the interface contract

A member is considered "already implemented" if:
- ✅ It exists with the correct signature and access modifier
- ✅ It properly satisfies the interface contract

---

## Test Cases

### M01.Method_AlreadyImplemented
**Input**: Class already implements `Method1()` correctly  
**Expected Output**: **EMPTY** (no code generated)  
**Why**: The method is already implemented, so nothing needs to be generated.

### M02.Method_FullGeneric
**Input**: Interface with fully generic method  
**Expected Output**: **GENERATES** the generic method  
**Why**: Generic methods need to be generated with proper type parameters.

### M03.Method_Generic
**Input**: Interface with generic method  
**Expected Output**: **GENERATES** the generic method  
**Why**: Generic methods need proper handling.

### M04.Method_ImplDetection_WrongParameterList
**Input**: Class has `Method1` but with wrong parameter types/order  
**Expected Output**: **GENERATES** the correct method  
**Why**: Wrong parameters = not implemented. The existing method doesn't satisfy the interface.

### M05.Method_ImplDetection_WrongParameterType
**Input**: Class has method with wrong parameter types  
**Expected Output**: **GENERATES** the correct method  
**Why**: Wrong parameter types = not implemented.

### M06.Method_NotImplemented
**Input**: Class does NOT implement `Method1()`  
**Expected Output**: **GENERATES** `public virtual int Method1() { return _sample.Method1(); }`  
**Why**: Method doesn't exist, so it must be generated.

### M07.Method_WithParameters
**Input**: Interface method with multiple parameters  
**Expected Output**: **GENERATES** method with all parameters passed through  
**Why**: Method doesn't exist, needs to be generated with proper parameter forwarding.

### M08.Multiple_Method_Parameter_Namespaces
**Input**: Methods with parameters from different namespaces  
**Expected Output**: **GENERATES** methods with proper using statements  
**Why**: Needs to handle namespace resolution for parameter types.

---

### P01.Parameter_Multiple_Types
**Input**: Interface with multiple properties  
**Expected Output**: **GENERATES** all unimplemented properties  
**Why**: Properties need to be generated if not implemented.

### P02.Property_AlreadyImplemented
**Input**: Class already implements `Prop1 { get; set; }` correctly  
**Expected Output**: **EMPTY** (no code generated)  
**Why**: Property is already implemented, so nothing needs to be generated.

### P03.Property_ImplDetection_WrongAccessModifier
**Input**: Class has `Prop1` but it's `protected` instead of `public`  
**Expected Output**: **GENERATES** `public virtual int Prop1 { get { return _sample.Prop1; } set { _sample.Prop1 = value; } }`  
**Why**: Wrong access modifier = not implemented. Interface requires `public`, but class has `protected`.

### P04.Property_ImplDetection_WrongReturnType
**Input**: Class has property with wrong return type  
**Expected Output**: **GENERATES** the correct property  
**Why**: Wrong return type = not implemented.

### P05.Property_NotImplemented
**Input**: Class does NOT implement `Prop1`  
**Expected Output**: **GENERATES** `public virtual int Prop1 { get { return _sample.Prop1; } set { _sample.Prop1 = value; } }`  
**Why**: Property doesn't exist, so it must be generated.

---

### C01.Multiple_Classes
**Input**: Two classes in different namespaces, both need `Method1`  
**Expected Output**: **GENERATES** code for BOTH classes (two separate files)  
**Why**: Each class needs its own generated code.

### C02.Target_Multiple_Types
**Input**: Class implements multiple interfaces  
**Expected Output**: **GENERATES** methods/properties for all unimplemented members  
**Why**: All interface members need to be satisfied.

---

### X01.Decoy_Attribute
**Input**: Class uses `[Decoy]` attribute instead of `[AutoCompose]`  
**Expected Output**: **EMPTY** (no code generated)  
**Why**: Only `[AutoCompose]` attribute triggers code generation. Other attributes are ignored.

### X02.Sample
**Input**: Standard sample case  
**Expected Output**: **GENERATES** unimplemented members  
**Why**: Check the specific interface members to see what needs generation.

### X03.Sample01
**Input**: Another sample case  
**Expected Output**: **GENERATES** unimplemented members  
**Why**: Check the specific interface members to see what needs generation.

---

### A01.Nameof_Target
**Input**: Uses `nameof(_sample)` instead of string literal  
**Expected Output**: **GENERATES** code normally  
**Why**: `nameof()` should be handled the same as string literals.

### A02.InternalClassGeneration
**Input**: Class with `internal` access modifier  
**Expected Output**: **GENERATES** code with `internal` modifier  
**Why**: Generated code should match the class's access modifier.

---

## Quick Decision Tree

When reviewing a `.received.cs` file, ask:

1. **Does the class have the `[AutoCompose]` attribute?**
   - ❌ No → Should be EMPTY
   - ✅ Yes → Continue

2. **For each interface member:**
   - **Does it exist in the class?**
     - ❌ No → **SHOULD GENERATE**
     - ✅ Yes → Continue
   
   - **Does it have the correct signature?**
     - ❌ No (wrong params, return type, or access modifier) → **SHOULD GENERATE**
     - ✅ Yes → **SHOULD NOT GENERATE** (already implemented)

3. **If generating, does it:**
   - Forward calls to `_sample` (or the specified member)?
   - Have correct access modifiers (`public virtual`)?
   - Include all necessary using statements?
   - Match the interface signature exactly?

---

## Common Patterns

### Empty Generated File
```csharp
namespace AutoCompose.Sample.XXX
{
    public partial class SampleXXX
    {
        // Empty - all members already implemented
    }
}
```
**Accept this when**: All interface members are already properly implemented.

### Method Generation
```csharp
public virtual int Method1(int param1, bool param2)
{
    return _sample.Method1(param1, param2);
}
```
**Accept this when**: Method doesn't exist or has wrong signature.

### Property Generation
```csharp
public virtual int Prop1 
{ 
    get { return _sample.Prop1; } 
    set { _sample.Prop1 = value; } 
}
```
**Accept this when**: Property doesn't exist or has wrong signature/access modifier.

