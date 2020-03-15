using Unity.Mathematics;
using Unity.Collections;

[Unity.Burst.BurstCompile]
public struct BlurRFloatJob : Unity.Jobs.IJobParallelFor
{
	[DeallocateOnJobCompletion][NativeDisableParallelForRestriction] NativeArray<RFloat> copy;
	readonly int Last;
	readonly int Width;
	NativeArray<RFloat> results;
	public BlurRFloatJob ( NativeArray<RFloat> data , int texture_width )
	{
		results = data;
		copy = new NativeArray<RFloat>( data , Allocator.TempJob );
		Last = results.Length-1;
		Width = texture_width;
	}
	void Unity.Jobs.IJobParallelFor.Execute ( int i )
	{
		const int kernelSize = 5;

		var px = copy[i];//center
		var pxr = copy[ math.min( i+1 , Last ) ];//right neighbour
		var pxl = copy[ math.clamp( i-1 , 0 , Last ) ];//left neighbour
		var pxt = copy[ math.clamp( i-Width , 0 , Last ) ];//top neighbour
		var pxb = copy[ math.min( i+Width , Last ) ];//bottom neighbour

		float R = ( ( px.R + pxr.R + pxl.R + pxt.R + pxb.R ) / kernelSize );
		
		results[i] = new RFloat{ R=R };
	}
}
