//   
// Copyright (c) Jesse Freeman, Pixel Vision 8. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) except for a few
// portions of the code. See LICENSE file in the project root for full 
// license information. Third-party libraries used by Pixel Vision 8 are 
// under their own licenses. Please refer to those libraries for details 
// on the license they use.
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christina-Antoinette Neofotistou @CastPixel
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany
//

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace PixelVision8.Engine.Utils
{
    
    public static class PixelDataUtil
    {
        private static int _mergeDestWidth;
        private static int _mergeDestHeight;
        private static int _mergeCol;
        private static int _mergeRow;
        private static int _mergeX;
        private static int _mergeY;

        public static int[] GetPixels(PixelData pixelData) => GetPixels(pixelData, 0, 0, pixelData.Width, pixelData.Height);
        
        public static int[] GetPixels(PixelData pixelData, int x, int y, int blockWidth, int blockHeight)
        {
            var tmpPixels = new int[blockWidth * blockHeight];
        
            CopyPixels(ref tmpPixels, pixelData, x, y, blockWidth, blockHeight);
        
            return tmpPixels;
        }
        
        public static void SetPixels(int[] pixels, PixelData pixelData) => SetPixels(pixelData, 0, 0, pixelData.Width, pixelData.Height, pixels);
        
        public static void SetPixels(PixelData pixelData, int x, int y, int blockWidth, int blockHeight, int[] srcPixels)
        {
            ValidateBounds(ref blockWidth, ref blockHeight, pixelData.Width, pixelData.Height, ref x, ref y);

            for (var i = blockHeight -1; i > -1; i--)
            {
                Array.Copy(
                    srcPixels, 
                    i  * blockWidth, 
                    pixelData.Pixels, 
                    x + (i + y) * pixelData.Width, 
                    blockWidth
                    );
            }

        }
        
        public static void CopyPixels(ref int[] destPixels, PixelData pixelData, int x, int y, int sampleWidth, int sampleHeight)
        {

            ValidateBounds(ref sampleWidth, ref sampleHeight, pixelData.Width, pixelData.Height, ref x, ref y);
            
            // Copy each entire line at once.
            for (var i = sampleHeight - 1; i > -1; --i)
            {
                
                Array.Copy(
                    pixelData.Pixels, 
                    x + (y + i) * pixelData.Width, 
                    destPixels, 
                    i * sampleWidth, 
                    sampleWidth
                );
                
            }
            
        }

        public static void Clear(PixelData pixelData,  int colorRef = -1)
        {
            for (var i = pixelData.TotalPixels - 1; i > -1; i--) pixelData[i] = colorRef;
        }

        public static void MergePixels(PixelData src, int sampleX, int sampleY, int sampleWidth, int sampleHeight,
            PixelData dest,
            int destX, int destY, bool flipH = false, bool flipV = false, int colorOffset = 0,
            bool ignoreTransparent = true)
        {

            _mergeDestWidth = dest.Width;
            _mergeDestHeight = dest.Height;
            
            ValidateBounds(ref sampleWidth, ref sampleHeight, _mergeDestWidth, _mergeDestHeight, ref destX, ref destY);

            var total = sampleWidth * sampleHeight;
            
            if(total == 0)
                return;

            _mergeCol = 0;
            _mergeRow = 0;    
            
            for (var i = 0; i < total; i++)
            {
                _mergeX = _mergeCol;
                _mergeY = _mergeRow;
                
                var index = (_mergeX + sampleX) + (src.Width * (_mergeY + sampleY));

                    var tmpPixel = src.Pixels[index];
                    
                    if (tmpPixel != -1 || ignoreTransparent != true)
                    {
                    
                        if (flipH) 
                            _mergeX = sampleWidth - 1 - _mergeX;

                        if (flipV) 
                            _mergeY = sampleWidth - 1 - _mergeY;

                        dest.Pixels[(_mergeX + destX) + _mergeDestWidth * (_mergeY + destY)] = tmpPixel + colorOffset;
                    }
                
                _mergeCol ++;
                
                if(_mergeCol >= sampleWidth) 
                {
                    _mergeCol = 0;
                    _mergeRow ++;
                }
            }
        }

        private static void ValidateBounds(ref int sampleWidth, ref int sampleHeight, int destWidth, int destHeight, ref int destX,
            ref int destY)
        {
            
            // Adjust X
            if (destX < 0)
            {
                sampleWidth += destX;
                destX = 0;
            }

            // Adjust Y
            if (destY < 0)
            {
                sampleHeight += destY;
                destY = 0;
            }

            // Adjust Width
            if (destX + sampleWidth > destWidth)
            {
                sampleWidth -= (destX + sampleWidth) - destWidth;
            }

            // Adjust Height.
            if (destY + sampleHeight > destHeight)
            {
                sampleHeight -= destY + sampleHeight - destHeight;
            }
        }

        public static void Resize(PixelData pixelData, int blockWidth, int blockHeight)
        {
            pixelData.Resize(MathHelper.Clamp(blockWidth, 1, 2048), MathHelper.Clamp(blockHeight, 1, 2048));
            
            Clear(pixelData);
        }

    }
    
}