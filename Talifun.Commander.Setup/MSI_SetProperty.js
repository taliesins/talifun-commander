// MSI_SetProperty.js <msi-file> <property> <value>
// Performs a post-build fixup of an msi to set the specified property (and add it if it doesn't already exist)

// Constant values from Windows Installer SDK
var msiOpenDatabaseModeTransact = 1;
var msiViewModifyInsert         = 1;
var msiViewModifyUpdate         = 2;  

if (WScript.Arguments.Length != 3)
{
    WScript.StdErr.WriteLine("Usage: " + WScript.ScriptName + "file property value");
    WScript.Quit(1);
}

var filespec = WScript.Arguments(0);
var property = WScript.Arguments(1);
var value = parseInt(WScript.Arguments(2));
var installer = WScript.CreateObject("WindowsInstaller.Installer");
var database = installer.OpenDatabase(filespec, msiOpenDatabaseModeTransact);

WScript.StdOut.WriteLine("Looking for property:" + property);

try
{   
    var sql = "SELECT Property, Value FROM Property WHERE Property = '" + property + "'";	
    var view = database.OpenView(sql);	
    view.Execute();		
    var record = view.Fetch();	

    if (record)
    {		
    	while (record)
    	{
    		WScript.StdOut.Write("Found: " + record.StringData(0) + ", " + record.StringData(1) + ", " + record.StringData(2));
    		if (record.IntegerData(2) != value)
    		{
    			WScript.StdOut.WriteLine(" - changing to " + value);
    			record.IntegerData(2) = value;
    			view.Modify(msiViewModifyUpdate,record);
    		}
    		else
    			WScript.StdOut.WriteLine(" - OK");

    		record = view.Fetch();
    	}
    }
    else
    {			
    	WScript.StdOut.WriteLine("Not found, so adding");
    	// There may be a better way to do this?
    	sql = "INSERT INTO Property (Property,Value) VALUES ('" + property + "','" + value + "')";
    	view = database.OpenView(sql);
    	view.Execute();		
    }
    view.Close();
    database.Commit();
}
catch(e)
{
    WScript.StdErr.WriteLine(e);
    WScript.Quit(1);
}