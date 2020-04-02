cd C:\Acumatica\AcumaticaTraining\KNCHANNELADVISOR\CstCustomizationUtil
CstCustomizatonUtil.exe -c CstCustomizatonUtil.ini -a MAKE_PACKAGE
CstCustomizatonUtil.exe -c CstCustomizatonUtil.ini -a UNPUBLISH_PACKAGE
CstCustomizatonUtil.exe -c CstCustomizatonUtil.ini -a UPLOAD_PACKAGE -p C:\Acumatica\AcumaticaTraining\KNCHANNELADVISOR\KChannelAdvisor.package\KChannelAdvisor.zip
CstCustomizatonUtil.exe -c CstCustomizatonUtil.ini -a PUBLISH_PACKAGE
cd ../
