// It uses for internal purpose only.

#ifndef __GCC_LINUX_H__
#define __GCC_LINUX_H__

#pragma once

#ifdef __cplusplus
extern "C" {
#endif

///////////////////////////////////////////////////
// Another standard C platform (gcc)

#if defined(__GNUC__) && defined(__linux__)

#if defined(__x86_64__) || defined(__i386__)
#include <x86intrin.h>
#elif defined(__ARM_NEON__)
#include <arm_neon.h>
#elif defined(__IWMMXT__)
#include <mmintrin.h>
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <wchar.h>
#include <malloc.h>
#define IL2C_USE_SIGNAL
#include <signal.h>

#include <unistd.h>

// Compatibility symbols (required platform depended functions)
extern wchar_t* il2c_itow(int32_t v, wchar_t* b, size_t l);
extern wchar_t* il2c_ultow(uint32_t v, wchar_t* b, size_t l);
extern wchar_t* il2c_i64tow(int64_t v, wchar_t* b, size_t l);
extern wchar_t* il2c_ui64tow(uint64_t v, wchar_t* b, size_t l);
#define il2c_snwprintf swprintf
#define il2c_wcstol wcstol
#define il2c_wcstoul wcstoul
#define il2c_wcstoll wcstoll
#define il2c_wcstoull wcstoull
#define il2c_wcstof wcstof
#define il2c_wcstod wcstod
#define il2c_wcscmp wcscmp
#define il2c_wcsicmp wcscasecmp
#define il2c_wcslen wcslen
#define il2c_check_heap()
#define il2c_malloc malloc

#if defined(_DEBUG)
extern void il2c_free(void* p);
#else
#define il2c_free free
#endif

#define il2c_mcalloc(name, size) \
    name = (((size) >= 256) ? il2c_malloc(size) : alloca(size)); \
    const bool is_##name##_heaped__ = ((size) >= 256)
#define il2c_mcfree(name) \
    do { if (is_##name##_heaped__) il2c_free(name); } while (0)

#define il2c_iand(pDest, newValue) __sync_fetch_and_and((interlock_t*)(pDest), (interlock_t)(newValue))
#define il2c_ior(pDest, newValue) __sync_fetch_and_or((interlock_t*)(pDest), (interlock_t)(newValue))
#define il2c_iinc(pDest) __sync_add_and_fetch((interlock_t*)(pDest), 1)
#define il2c_idec(pDest) __sync_sub_and_fetch((interlock_t*)(pDest), 1)
#define il2c_ixchg(pDest, newValue) __sync_lock_test_and_set((interlock_t*)(pDest), (interlock_t)(newValue))
#define il2c_ixchgptr(ppDest, pNewValue) __sync_lock_test_and_set((void**)(ppDest), (void*)(pNewValue))
#define il2c_icmpxchg(pDest, newValue, comperandValue) __sync_val_compare_and_swap((interlock_t*)(pDest), (interlock_t)(comperandValue), (interlock_t)(newValue))
#define il2c_icmpxchgptr(ppDest, pNewValue, pComperandValue) __sync_val_compare_and_swap((void**)(ppDest), (void*)(pComperandValue), (void*)(pNewValue))
extern void il2c_sleep(uint32_t milliseconds);
#define il2c_longjmp longjmp

#endif

#ifdef __cplusplus
}
#endif

#endif
