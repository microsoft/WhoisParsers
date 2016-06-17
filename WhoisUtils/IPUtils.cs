// -----------------------------------------------------------------------
// <copyright file="IPUtils.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Utils
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Numerics;

    public static class IPUtils
    {
        public static IPAddress Increment(this IPAddress address)
        {
            return address.IncrementBy(1);
        }

        public static IPAddress IncrementBy(this IPAddress address, int incrementValue)
        {
            return IncrementBy(address, new BigInteger(incrementValue));
        }

        public static IPAddress IncrementBy(this IPAddress address, BigInteger incrementValue)
        {
            // As an example we will use the input address = IPAddress.Parse("192.168.0.1") and incrementValue = 1
            int addressBytesLength;
            var addressNumeric = address.ToBigInteger(out addressBytesLength); // Example: addressNumeric = 3232235521 (which is the correct numeric representation of the 192.168.0.1 IP address) and addressBytesLength = 4

            // Increment address by incrementValue
            addressNumeric += incrementValue; // Example: addressNumeric = 3232235522 (which is the correct numeric representation of the 192.168.0.2 IP address)

            var newAddressBytes = addressNumeric.ToByteArray(); // Example: addressNumeric = [2, 0, 168, 192, 0]

            // The number of bytes returned by the addressNumeric.ToByteArray() function varies depending on how large the number if
            // However, we need to copy everything in an array that is the size of the original input.
            // This step also has the side effect of removing the extra 0 byte if the BigInteger output array is too long.
            var trimmedNewAddressBytes = new byte[addressBytesLength];
            var copyLength = newAddressBytes.Length < addressBytesLength ? newAddressBytes.Length : addressBytesLength;
            Array.Copy(newAddressBytes, trimmedNewAddressBytes, copyLength); // Example: trimmedNewAddressBytes = [2, 0, 168, 192]

            Array.Reverse(trimmedNewAddressBytes); // Example: trimmedNewAddressBytes = [192, 168, 0, 2]
            return new IPAddress(trimmedNewAddressBytes);
        }

        public static bool IsLessThanOrEqual(this IPAddress thisAddress, IPAddress comparisonAddress)
        {
            return thisAddress.ToBigInteger() <= comparisonAddress.ToBigInteger();
        }

        public static bool IsLessThan(this IPAddress thisAddress, IPAddress comparisonAddress)
        {
            return thisAddress.ToBigInteger() < comparisonAddress.ToBigInteger();
        }

        public static bool IsGreaterThanOrEqual(this IPAddress thisAddress, IPAddress comparisonAddress)
        {
            return thisAddress.ToBigInteger() >= comparisonAddress.ToBigInteger();
        }

        public static bool IsGreaterThan(this IPAddress thisAddress, IPAddress comparisonAddress)
        {
            return thisAddress.ToBigInteger() >= comparisonAddress.ToBigInteger();
        }

        public static BigInteger ToBigInteger(this IPAddress address)
        {
            int addressBytesLength;
            return address.ToBigInteger(out addressBytesLength);
        }

        public static BigInteger ToBigInteger(this IPAddress address, out int addressBytesLength)
        {
            // As an example we will use the input address = IPAddress.Parse("192.168.0.1")

            // These bytes are in network byte order which has the most significant byte first (big endian)
            var addressBytes = address.GetAddressBytes(); // Example: addressBytes = [192, 168, 0, 1]
            addressBytesLength = addressBytes.Length; // Example: addressBytesLength = 4

            // Convert bytes to little endian, since BigInteger expects that
            Array.Reverse(addressBytes); // Example: addressBytes = [1, 0, 168, 192]

            // http://stackoverflow.com/questions/5649190/byte-to-unsigned-biginteger - "The remarks for the BigInteger constructor state that you can make sure any BigInteger created from a byte[] is unsigned if you append a 00 byte to the end of the array before calling the constructor."
            var forceUnsignedBytes = new byte[addressBytesLength + 1];
            Array.Copy(addressBytes, forceUnsignedBytes, addressBytesLength);
            forceUnsignedBytes[addressBytesLength] = (byte)0; // Example: forceUnsignedBytes = [1, 0, 168, 192, 0]

            // BigInteger expects the byte array to be in little endian format (stores the least significant byte at the first location)
            var addressNumeric = new BigInteger(forceUnsignedBytes); // Example: addressNumeric = 3232235521 (which is the correct numeric representation of the 192.168.0.1 IP address)

            return addressNumeric;
        }
    }
}
