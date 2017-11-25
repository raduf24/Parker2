using System;
using System.Drawing;
using System.Drawing.Imaging;


public class ImageOperations
{
    public void SetMaskColor(Bitmap target, byte r, byte g, byte b)
    {
        BitmapData targetData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);                
        var targetPixels = GetBitmapDataArray(targetData);

        for (int p = 0; p < targetPixels.Length; p += 4)
        {
            if (targetPixels[p+1] != 0)
            {
                
                targetPixels[p] = b;
                targetPixels[p + 1] = g;
                targetPixels[p + 2] = r;
                targetPixels[p + 3] = 255;
            }
        }

        SetBitmapDataArray(targetData, targetPixels);
        target.UnlockBits(targetData);
    }

    public long SumAllRGB(Bitmap target)
    {
        long res = 0;
        BitmapData targetData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        var targetPixels = GetBitmapDataArray(targetData);
        target.UnlockBits(targetData);

        for (int p = 0; p < targetPixels.Length; p ++)
        {
            res += targetPixels[p];              
        }        
        return res;
    }

    public int CountNonEmptyPixels(Bitmap target)
    {
        int res = 0;
        BitmapData targetData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        var targetPixels = GetBitmapDataArray(targetData);
        target.UnlockBits(targetData);

        for (int p = 0; p < targetPixels.Length; p += 3)
        {
            if (targetPixels[p] != 0 || targetPixels[p + 1] != 0 || targetPixels[p + 2] != 0)
                res++;
        }        
        return res;
    }
    public void DiffImage(Bitmap target, Bitmap operand)
    {
        BitmapData targetData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        BitmapData operandData = operand.LockBits(new Rectangle(0, 0, operand.Width, operand.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

        var operandPixels = GetBitmapDataArray(operandData);
        operand.UnlockBits(operandData);

        var targetPixels = GetBitmapDataArray(targetData);

        for (int p = 0; p < targetPixels.Length; p += 3)
        {
            targetPixels[p] =(byte)Math.Abs((int)targetPixels[p] - (int)operandPixels[p]);
            targetPixels[p + 1] = (byte)Math.Abs((int)targetPixels[p + 1] - (int)operandPixels[p + 1]);
            targetPixels[p + 2] = (byte)Math.Abs((int)targetPixels[p + 2] - (int)operandPixels[p + 2]);
        }

        SetBitmapDataArray(targetData, targetPixels);
        target.UnlockBits(targetData);
    }

    public void CropImageWithMask(Bitmap target, Bitmap mask)
    {
        BitmapData targetData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        BitmapData maskData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

        var maskPixels = GetBitmapDataArray(maskData);
        mask.UnlockBits(maskData);

        var targetPixels = GetBitmapDataArray(targetData);

        for (int p = 0; p < targetPixels.Length; p += 3)
        {
            if (maskPixels[p] == 0)
            {
                targetPixels[p] = targetPixels[p + 1] = targetPixels[p + 2] = 0;
            }
        }

        SetBitmapDataArray(targetData, targetPixels);
        target.UnlockBits(targetData);
    }

    private byte[] GetBitmapDataArray(BitmapData data)
    {
        IntPtr ptr = data.Scan0;
        int bytes = Math.Abs(data.Stride) * data.Height;
        byte[] pixelValues = new byte[bytes];
        System.Runtime.InteropServices.Marshal.Copy(ptr, pixelValues, 0, bytes);
        return pixelValues;
    }

    private void SetBitmapDataArray(BitmapData data, byte[] pixelValues)
    {
        IntPtr ptr = data.Scan0;
        int bytes = Math.Abs(data.Stride) * data.Height;
        System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, ptr, bytes);
    }
}