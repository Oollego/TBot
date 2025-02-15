using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class PhpGeneratorService: IPhpGeneratorService
    {
        public string GeneratePhpScript(string appName, string appBundle, string secret, string secretKeyParam)
        {
            return $@"<?php
                $appName = '{appName}';
                $appBundle = '{appBundle}';
                $secretKey = '{secret}';
                if($secretKey == $_GET['{secretKeyParam}']){{echo 'Привет я приложение ' . $appName . ' моя ссылка на гугл плей https://play.google.com/store/apps/details?id=' . $appBundle ;}}";
        }
    }
}
    
