//
// StencilMaskAllocator.cs
//
// Projector For LWRP
//
// Copyright (c) 2020 NYAHOON GAMES PTE. LTD.
//

namespace ProjectorForLWRP
{
	public static class StencilMaskAllocator
	{
		const int STENCIL_BIT_COUNT = 8;
		private static int s_availableBits = 0xFF;
		private static int s_allocateCount = 0;
		private static bool s_loopFlag = false;
		public static void Init(int mask)
		{
			s_availableBits = mask & ((1 << STENCIL_BIT_COUNT) - 1);
			s_allocateCount = -1;
			s_loopFlag = false;
			MoveNext();
		}
		public static int AllocateSingleBit()
		{
			MoveNext();
			return GetCurrentBit();
		}
		public static int GetCurrentBit()
		{
			return (1 << s_allocateCount) & s_availableBits;
		}
		public static int availableBits { get { return s_availableBits; } }
		public static bool loopFlag { get { return s_loopFlag; } }
		public static void ClearLoopFlag()
		{
			s_loopFlag = false;
		}
		private static void MoveNext()
		{
			if (s_availableBits != 0)
			{
				do
				{
					++s_allocateCount;
					if (s_allocateCount == STENCIL_BIT_COUNT)
					{
						s_loopFlag = true;
						s_allocateCount = 0;
					}
				} while ((s_availableBits & (1 << s_allocateCount)) == 0);
			}
		}
	}
}
