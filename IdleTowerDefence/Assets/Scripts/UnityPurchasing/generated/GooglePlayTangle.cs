// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("WCzdSiyA0k3vjsEZ+xpcHe5VEFk5B2/q+/GZm++cf4Goo8mzH5xsVajgsJ32mRQ0R9YIYZlmtra2B30kpyQqJRWnJC8npyQkJZ5lzSqjbYXKey2BHKdYCZPnSRCKJLOtDooEyKEUQq4SKJqsx0sfWvkIPz1b+ZmvP9c/PpZ2YDJDClgH1PY6lkV9xIJg3hsoj4yXvpVEBbJO0D5WCWyl57AUNlD/qsDhBwc3j7J0xHbMOC7Y82wnnCo0WAEjjX/IIyFGL/WWXqsGxYcIYsLLnLWVhsGmUnzqLTJSbTxfpl5GfRHfG0qa19iEScLzkU3hFackBxUoIywPo22j0igkJCQgJSY5xksXZ72O4k9iw1BvHu1dp5QvkcibxMEO8aNhUCcmJCUk");
        private static int[] order = new int[] { 9,3,2,3,8,7,8,13,10,10,13,11,13,13,14 };
        private static int key = 37;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
