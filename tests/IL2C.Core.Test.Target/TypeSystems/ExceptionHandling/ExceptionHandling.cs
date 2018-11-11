﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IL2C.TypeSystems
{
    [TestCase(123, "RaiseAndCaughtLocal", false)]
    [TestCase(456, "RaiseAndCaughtLocal", true)]
    [TestCase("ABC", "RaiseCaughtAndAccessLocal", "ABC")]
    [TestCase(null, "RaiseCaughtAndAccessLocal", null)]
    [TestCase("ABC", "RaiseCaughtNarrowingLocal", "ABC")]
    [TestCase(null, "RaiseCaughtNarrowingLocal", null)]
    [TestCase("ABC", "RaiseCaughtWildcardLocal", "ABC")]
    [TestCase(null, "RaiseCaughtWildcardLocal", null)]
    [TestCase(123, "RaiseAndCaughtMultipleHandlerLocal", 0)]
    [TestCase(456, "RaiseAndCaughtMultipleHandlerLocal", 1)]
    [TestCase(789, "RaiseAndCaughtMultipleHandlerLocal", 2)]
    [TestCase(29, "FinallyWithNonThrowingLocal", 123)]
    [TestCase(129, "FinallyWithThrowingAndNestedLocal", 123)]
    [TestCase(129, "FinallyAndCaughtWithThrowingAndNestedLocal", 123, true)]
    [TestCase(29, "FinallyAndCaughtWithThrowingAndNestedLocal", 123, false)]
    [TestCase(123, "RaiseAndNestedCaughtLocal", 123)]
    [TestCase(123, "RaiseAndNestedCaughtOuterLocal", 123)]
    [TestCase(223, "RaiseCaughtAndRethrowLocal", 123)]
    [TestCase(223, "RaiseCaughtAndThrowNewExceptionLocal", 123)]
    [TestCase(123, new[] { "RaiseAndCaughtGlobal", "RaiseException" }, false)]
    [TestCase(456, new[] { "RaiseAndCaughtGlobal", "RaiseException" }, true)]
    public sealed class ExceptionHandling
    {
        public static int RaiseAndCaughtLocal(bool sw)
        {
            try
            {
                if (sw) throw new Exception();
            }
            catch (Exception)
            {
                return 456;
            }
            return 123;
        }

        public static string RaiseCaughtAndAccessLocal(string value)
        {
            try
            {
                if (value != null) throw new Exception(value);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        public static string RaiseCaughtNarrowingLocal(string value)
        {
            try
            {
                if (value != null) throw new InvalidCastException(value);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        public static string RaiseCaughtWildcardLocal(string value)
        {
            try
            {
                if (value != null) throw new Exception();
            }
            catch
            {
                return value;
            }
            return null;
        }

        public static int RaiseAndCaughtMultipleHandlerLocal(int sw)
        {
            try
            {
                if (sw == 1) throw new Exception();
                if (sw == 2) throw new InvalidCastException();
            }
            catch (InvalidCastException)
            {
                return 789;
            }
            catch (Exception)
            {
                return 456;
            }
            return 123;
        }

        public static int FinallyWithNonThrowingLocal(int value)
        {
            int r = 0;
            try
            {
                r += value;
            }
            finally
            {
                r %= 47;
            }

            return r;
        }

        public static int FinallyWithThrowingAndNestedLocal(int value)
        {
            int r = value;
            try
            {
                try
                {
                    throw new Exception();
                }
                finally
                {
                    r %= 47;
                }
            }
            catch (Exception)
            {
                r += 100;
            }

            return r;
        }

        public static int FinallyAndCaughtWithThrowingAndNestedLocal(int value, bool rethrow)
        {
            int r = 0;
            try
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception)
                {
                    r += value;
                    if (rethrow) throw;
                }
                finally
                {
                    r %= 47;
                }
            }
            catch (Exception)
            {
                r += 100;
            }

            return r;
        }

        public static int RaiseAndNestedCaughtLocal(int value)
        {
            int r = 0;
            try
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception)
                {
                    r += value;
                }
            }
            catch (Exception)
            {
                r += 100;
            }

            return r;
        }

        public static int RaiseAndNestedCaughtOuterLocal(int value)
        {
            int r = 0;
            try
            {
                try
                {
                    throw new Exception();
                }
                catch (InvalidCastException)
                {
                    r += 100;
                }
            }
            catch (Exception)
            {
                r += value;
            }

            return r;
        }

        public static int RaiseCaughtAndRethrowLocal(int value)
        {
            int r = 0;
            try
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception)
                {
                    r += 100;
                    throw;
                }
            }
            catch (Exception)
            {
                r += value;
            }

            return r;
        }

        public static int RaiseCaughtAndThrowNewExceptionLocal(int value)
        {
            int r = 0;
            try
            {
                try
                {
                    throw new Exception();
                }
                catch (Exception)
                {
                    r += 100;
                    throw new InvalidCastException();
                }
            }
            catch (InvalidCastException)
            {
                r += value;
            }

            return r;
        }

        private static void RaiseException(bool sw)
        {
            if (sw) throw new Exception();
        }

        public static int RaiseAndCaughtGlobal(bool sw)
        {
            try
            {
                RaiseException(sw);
            }
            catch (Exception)
            {
                return 456;
            }
            return 123;
        }
    }
}
