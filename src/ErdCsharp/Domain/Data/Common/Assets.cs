using System;
using System.Collections.Generic;
using System.Text;
using ErdCsharp.Provider.Dtos.API.Common;

namespace ErdCsharp.Domain.Data.Common
{
    public class Assets
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] Tags { get; private set; }
        public string IconPng { get; private set; }
        public string IconSvg { get; private set; }

        private Assets() { }

        public static Assets From(AssetsDto assets)
        {
            if (assets == null) return null;

            return new Assets()
            {
                Name = assets.Name,
                Description = assets.Description,
                Tags = assets.Tags,
                IconPng = assets.IconPng,
                IconSvg = assets.IconSvg,
            };
        }
    }
}
