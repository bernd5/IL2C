﻿.class public IL2C.ILConverters.Call_Overload_Newslot_Virtual
{
	.method public static string Instance_Overload_Newslot_Virtual_ToString_System_Object() cil managed
	{
		.maxstack 1
        newobj instance void class IL2C.ILConverters.Call_Overload_Newslot_Virtual::.ctor()
        call instance string [mscorlib]System.Object::ToString()
		ret
	}

	.method public static string Instance_Overload_Newslot_Virtual_ToString_NoArgument_IL2C_ILConverters_Call() cil managed
	{
		.maxstack 1
        newobj instance void class IL2C.ILConverters.Call_Overload_Newslot_Virtual::.ctor()
        call instance string IL2C.ILConverters.Call_Overload_Newslot_Virtual::ToString()
		ret
	}

	.method public static string Instance_Overload_Newslot_Virtual_ToString_IL2C_ILConverters_Call(string v) cil managed
	{
		.maxstack 2
        newobj instance void class IL2C.ILConverters.Call_Overload_Newslot_Virtual::.ctor()
		ldarg.0
        call instance string IL2C.ILConverters.Call_Overload_Newslot_Virtual::ToString(string v)
		ret
	}
}
