using ByteBrewSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDK : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MaxSdk.SetSdkKey("aphd8il0_BBgCTiXvRcq_UivoZJsUG25nIEVaGLPTOPMv3HPApniq8aw1tVlHbA60m0CoEts92UpxRxeDerKq1");
        MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        MaxSdk.SetVerboseLogging(true);
        MaxSdk.InitializeSdk();


        ByteBrew.InitializeByteBrew();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
