﻿.class public IL2C.ILConverters.Callvirt_Derived2_Newslot_Virtual
{
    .method public static string Derived2_Newslot_Virtual_ToString_System_Object() cil managed
    {
        .maxstack 1
        newobj instance void class IL2C.ILConverters.Callvirt_Derived2_Newslot_Virtual::.ctor()
        callvirt instance string [mscorlib]System.Object::ToString()
        ret
    }

    .method public static string Derived2_Newslot_Virtual_ToString_IL2C_ILConverters_Callvirt_Base() cil managed
    {
        .maxstack 1
        newobj instance void class IL2C.ILConverters.Callvirt_Derived2_Newslot_Virtual::.ctor()
        callvirt instance string IL2C.ILConverters.Callvirt_Derived2_Newslot_Virtual_Base::ToString()
        ret
    }

    .method public static string Derived2_Newslot_Virtual_ToString_IL2C_ILConverters_Callvirt() cil managed
    {
        .maxstack 1
        newobj instance void class IL2C.ILConverters.Callvirt_Derived2_Newslot_Virtual::.ctor()
        callvirt instance string IL2C.ILConverters.Callvirt_Derived2_Newslot_Virtual::ToString()
        ret
    }
}
