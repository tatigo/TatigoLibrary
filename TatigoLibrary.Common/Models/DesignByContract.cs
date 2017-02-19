using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TatigoLibrary.Common.DesignByContract
{
    public class DesignByContractException : ApplicationException
    {
        protected DesignByContractException()
        {
        }

        protected DesignByContractException(string message) : base(message)
        {
        }

        protected DesignByContractException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class AssertionException : DesignByContractException
    {
        public AssertionException()
        {
        }

        public AssertionException(string message) : base(message)
        {
        }

        public AssertionException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class InvariantException : DesignByContractException
    {
        public InvariantException()
        {
        }

        public InvariantException(string message) : base(message)
        {
        }

        public InvariantException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class PostconditionException : DesignByContractException
    {
        public PostconditionException()
        {
        }

        public PostconditionException(string message) : base(message)
        {
        }

        public PostconditionException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class PreconditionException : DesignByContractException
    {
        public PreconditionException()
        {
        }

        public PreconditionException(string message) : base(message)
        {
        }

        public PreconditionException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public sealed class Check
    {
        private static bool useAssertions;

        public static bool UseAssertions
        {
            get
            {
                return Check.useAssertions;
            }
            set
            {
                Check.useAssertions = value;
            }
        }

        private static bool UseExceptions
        {
            get
            {
                return !Check.useAssertions;
            }
        }

        private Check()
        {
        }

        public static void Assert(bool assertion, string message)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Assertion: ", message));
            }
            else if (!assertion)
            {
                throw new AssertionException(message);
            }
        }

        public static void Assert(bool assertion, string message, Exception inner)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Assertion: ", message));
            }
            else if (!assertion)
            {
                throw new AssertionException(message, inner);
            }
        }

        public static void Assert(bool assertion)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, "Assertion failed.");
            }
            else if (!assertion)
            {
                throw new AssertionException("Assertion failed.");
            }
        }

        public static void Ensure(bool assertion, string message)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Postcondition: ", message));
            }
            else if (!assertion)
            {
                throw new PostconditionException(message);
            }
        }

        public static void Ensure(bool assertion, string message, Exception inner)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Postcondition: ", message));
            }
            else if (!assertion)
            {
                throw new PostconditionException(message, inner);
            }
        }

        public static void Ensure(bool assertion)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, "Postcondition failed.");
            }
            else if (!assertion)
            {
                throw new PostconditionException("Postcondition failed.");
            }
        }

        public static void Invariant(bool assertion, string message)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Invariant: ", message));
            }
            else if (!assertion)
            {
                throw new InvariantException(message);
            }
        }

        public static void Invariant(bool assertion, string message, Exception inner)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Invariant: ", message));
            }
            else if (!assertion)
            {
                throw new InvariantException(message, inner);
            }
        }

        public static void Invariant(bool assertion)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, "Invariant failed.");
            }
            else if (!assertion)
            {
                throw new InvariantException("Invariant failed.");
            }
        }

        public static void Require(bool assertion, string message)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Precondition: ", message));
            }
            else if (!assertion)
            {
                throw new PreconditionException(message);
            }
        }

        public static void Require(bool assertion, string message, Exception inner)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, string.Concat("Precondition: ", message));
            }
            else if (!assertion)
            {
                throw new PreconditionException(message, inner);
            }
        }

        public static void Require(bool assertion)
        {
            if (!Check.UseExceptions)
            {
                Trace.Assert(assertion, "Precondition failed.");
            }
            else if (!assertion)
            {
                throw new PreconditionException("Precondition failed.");
            }
        }
    }
}
