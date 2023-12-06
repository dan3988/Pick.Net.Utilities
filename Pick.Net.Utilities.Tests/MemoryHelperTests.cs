namespace Pick.Net.Utilities.Tests;

[TestClass]
public class MemoryHelperTests
{
	[TestMethod]
	public unsafe void TestToLowerHexString()
	{
		var ints = new int[32] { 259390707, -1740967044, 1234195351, 1031597702, 1573798440, 523033782, -1271843631, 2005299527, -571310635, -927192333, 2097694592, -1983472446, -220319068, -339114886, -1027093188, 1133767270, 423691407, -2087853970, -64185472, 307596958, 1251798245, -674630066, -56228208, 1650251466, 1072063269, -1019632477, 2133491029, 1319884801, 1755179421, -1263006380, -503803374, -303496711 };
		var test = ints.ToHexString();

		Assert.AreEqual("f3fc750f7cf33a989753904986ee7c3d2842ce5db6dc2c1fd13431b447718677d57df2ddf32abcc88047087dc29cc689a432def27a84c9eb3ccdc7c266ea93438f0441196ee08d83809b2cfc9e8e5512e5ec9c4a4ef6c9d79006a6fccad65c622563e63fa3a439c3557d2a7f01d8ab4e9de99d68540db8b41292f8e1f901e9ed", test);
	}

	[TestMethod]
	public unsafe void TestToUpperHexString()
	{
		var ints = new uint[32] { 1461580784, 1105619885, 3700677864, 909041098, 1907495575, 2988732209, 1769818132, 4130290751, 731328807, 1715281563, 497698347, 2806079540, 1821845475, 3151951769, 3043703239, 4154150705, 4263615811, 2550650976, 715319982, 3968581265, 273982066, 1556755703, 778413581, 2497589477, 3631168275, 3568630673, 3779296021, 716389812, 4123025742, 1500214052, 425243910, 1887026473 };
		var test = ints.ToHexString(true);

		Assert.AreEqual("F0F31D57AD6BE641E8DC93DCCADD2E369712B271316F24B214487D693F3C2FF62731972B9B1E3D662B46AA1D346041A7E327976C99F7DEBBC7396BB5314F9BF7439D21FE60D80798AEEAA22A91BE8BEC72A25410F734CA5C0DA6652EE530DE94133B6FD891FBB4D4157B43E1B43DB32A4E61C0F524736B5906B5581929BD7970", test);
	}

	[TestMethod]
	public unsafe void TestToBinaryString()
	{
		var ints = new byte[32] { 236, 2, 133, 150, 204, 0, 131, 218, 177, 75, 234, 196, 235, 8, 11, 206, 104, 108, 163, 244, 25, 138, 122, 112, 9, 105, 255, 188, 90, 39, 202, 101 };
		var test = ints.ToBinaryString();

		Assert.AreEqual("0011011101000000101000010110100100110011000000001100000101011011100011011101001001010111001000111101011100010000110100000111001100010110001101101100010100101111100110000101000101011110000011101001000010010110111111110011110101011010111001000101001110100110", test);
	}
}