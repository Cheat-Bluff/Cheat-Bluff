﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPro.Hero.Browser.Media;

namespace GoPro.Hero.Browser.Media
{
    public static class ImageExtensions
    {
        public static async Task<Stream> BigThumbnailAsync(this Image image)
        {
            return await image.BigThumbnailAsync(image.Name);
        }

        public static async Task<IMediaBrowser> DeleteAsync(this Image image)
        {
            await image.DeleteFile(image.Name);
            return image.Browser;
        }
    }
}
