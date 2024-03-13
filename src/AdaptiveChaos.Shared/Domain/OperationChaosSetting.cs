using System.Net;

namespace AdaptiveChaos.Shared.Domain
{
    public class OperationChaosSetting
    {
        private static readonly TimeSpan NoLatency = TimeSpan.Zero;
        private static readonly bool NotEnabled = false;
        private static readonly double NoInjectionRate = 0;
        private static readonly Exception NoExceptionResult = null;
        private static readonly HttpResponseMessage NoHttpResponse = null;

        public OperationChaosSetting()
        {
            this.Enabled = true;
        }

        public bool Enabled { get; set; }

        public double InjectionRate { get; set; }

        public int StatusCode { get; set; }

        public int LatencyMs { get; set; }

        public string Exception { get; set; }

        public TimeSpan GetLatency()
        {

            if (this.LatencyMs <= 0) return NoLatency;

            return TimeSpan.FromMilliseconds(this.LatencyMs);
        }

        public double GetInjectionRate()
        {
            //todo validation for injection rate for 0 - 1
            if (this.InjectionRate <= 0 || this.InjectionRate > 1)
                return NoInjectionRate;

            return this.InjectionRate;
        }

        public bool GetEnabled()
        {
            return this.Enabled;
        }

        public HttpResponseMessage GetHttpResponseMessage()
        {

            if (this.StatusCode < 200) return NoHttpResponse;

            try
            {
                return new HttpResponseMessage((HttpStatusCode)this.StatusCode);
            }
            catch
            {
                return NoHttpResponse;
            }
        }

        public Exception GetException()
        {

            if (string.IsNullOrWhiteSpace(this.Exception)) return NoExceptionResult;

            try
            {
                Type exceptionType = Type.GetType(this.Exception);
                if (exceptionType == null) return NoExceptionResult;

                if (!typeof(Exception).IsAssignableFrom(exceptionType)) return NoExceptionResult;

                var instance = Activator.CreateInstance(exceptionType);
                var result = instance as Exception;

                return result;
            }
            catch
            {
                return NoExceptionResult;
            }
        }

        public bool GetHttpResponseEnabled()
        {
            if (GetHttpResponseMessage() == NoHttpResponse)
                return NotEnabled;

            return GetEnabled();
        }

        public bool GetExceptionEnabled()
        {
            if (GetException() == NoExceptionResult)
                return NotEnabled;

            return GetEnabled();
        }

        public bool GetLatencyEnabled()
        {
            return this.LatencyMs >= 0;
        }
    }
}
