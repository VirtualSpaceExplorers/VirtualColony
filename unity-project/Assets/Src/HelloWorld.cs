using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

	static void PerformBuild ()
	{
		Console.WriteLine (":: Performing build");
		var buildTarget = GetBuildTarget ();
		var buildPath = GetBuildPath ();
		var buildName = GetBuildName ();
		var fixedBuildPath = GetFixedBuildPath (buildTarget, buildPath, buildName);
        	Console.WriteLine("Fixed build path:" + fixedBuildPath);
        	var buildInfo = BuildPipeline.BuildPlayer (GetEnabledScenes (), fixedBuildPath, buildTarget, GetBuildOptions ());
		if( buildInfo.summary.result == BuildResult.Succeeded ) {
			Console.WriteLine (":: Done with build");
		}
		else {
			Console.WriteLine (":: Build error");
            foreach (var step in buildInfo.steps) {
                foreach (var message in step.messages) {
			        Console.WriteLine (step.name + " -- " + message.content);
                }
            }
    		EditorApplication.Exit( 1 );
		}
	}
