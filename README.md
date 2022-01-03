# RedMoonConnect

Connect to custom DAOC Freeshard with the launcher tool and Avalonia GUI.<br>

![launcher-1](https://user-images.githubusercontent.com/57635141/147961932-a5c8c0ca-feb3-4367-a8f0-8c7c1b87b41a.png)


# Generate solution

Open the solution in Visual Build and proceed with the build.<br>
Note: the connect.exe will be copied after compilation in the destination folder (ie CopyCustomContent in csproj file)

# Launch 

Once the solution has been generated, you can run the launcher.<br>
Note : game.dll (or game-1125.dll) is not provided and you will need to copy it from your DAOC folder<br>

For Windows users : <br>
  - There's no need to use wine option, you only need to provide login/password to connect to the freeshard.
  - You can tick the option "customized settings" and specify the DAOC folder if needed. You can also specify a customized freeshard address

For Mac/Linux users : <br>
  - You will need to tick the wine option
  - Default folder which will be used is ".wine" and you can specify the wine command name (default is "wine", but you can specify for instance "wine32on64")
 
# Packaging

For Windows : <br>
  - A ZIP file containing the solution should be enough to distribute the solution

For Mac/Linux : <br>
  - You will need to provide the solution in a folder named "connect-tool" and put in the parent directory the script named "PlayRedMoon.command" from the Assets directory : this script will change the directory to the "connect-tool" folder before launching the RedMoonConnect executable


