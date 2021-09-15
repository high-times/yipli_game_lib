#import <Foundation/Foundation.h>
#import "BLE.h"
#import "BLEFramework.h"
#import "parseHexString.m"
#import "DriverControl.h"
#import <string>
NSString *const BLEUnityMessageName_OnBleDidInitialize = @"OnBleDidInitializeMessage";
NSString *const BLEUnityMessageName_OnBleDidConnect = @"OnBleDidConnectMessage";
NSString *const BLEUnityMessageName_OnBleDidCompletePeripheralScan = @"OnBleDidCompletePeripheralScanMessage";
NSString *const BLEUnityMessageName_OnBleDidDisconnect = @"OnBleDidDisconnectMessage";
NSString *const BLEUnityMessageName_OnBleDidReceiveData = @"OnBleDidReceiveDataMessage";

@interface BLEFrameworkDelegate() <BLEDelegate>
@property (strong,nonatomic) NSMutableArray *mDevices;
@property (strong,nonatomic) NSMutableArray *mDevicesObject;

@property (strong, nonatomic) BLE *ble;
@property (nonatomic) BOOL isConnected;
@property (nonatomic) BOOL searchDevicesDidFinish;



@end

@implementation BLEFrameworkDelegate
std::string FMResponse;

DriverControl d1(1);

std::unique_ptr<DriverControl> d2;

- (id) init
{
    self = [super init];
    if (self)
    {
        self.ble = [[BLE alloc] init];
        
        if (self.ble)
        {
            [self.ble controlSetup];
            self.ble.delegate = self;
            self.mDevices = [[NSMutableArray alloc] init];
            self.mDevicesObject = [[NSMutableArray alloc] init];
            
        }
        else
        {
            [BLEFrameworkDelegate SendUnityMessage:BLEUnityMessageName_OnBleDidInitialize message:@"Fail"];
        }
    }
    else
    {
        [BLEFrameworkDelegate SendUnityMessage:BLEUnityMessageName_OnBleDidInitialize message:@"Fail"];
    }
    
    return self;
}



#pragma mark BLEDelegate methods

-(void) bleDidConnect
{
    NSLog(@"->Connected");
    self.isConnected = true;
    
  
    
    [BLEFrameworkDelegate SendUnityMessage:BLEUnityMessageName_OnBleDidConnect message:@"Success"];
    
    // Schedule to read RSSI every 1 sec.
    //rssiTimer = [NSTimer scheduledTimerWithTimeInterval:(float)1.0 target:self selector:@selector(readRSSITimer:) userInfo:nil repeats:YES];
}

- (void)bleDidDisconnect
{
    self.isConnected = false;
    
    [BLEFrameworkDelegate SendUnityMessage:BLEUnityMessageName_OnBleDidDisconnect message:@"Success"];

    
    //[rssiTimer invalidate];
}

- (void)bleDidUpdateRSSI:(NSNumber *) rssi
{
    //self.rssiValue = rssi.stringValue;
}

- (void) bleDidReceiveData:(unsigned char *)data length:(int)length
{
    if (length > 0 && data != NULL) {
        
        _dataRx = [NSData dataWithBytes:data length:length];
      
        NSString* hex = [_dataRx hexRepresentationWithSpaces_AS:NO];
    
        if (DriverControl::gameMode == SINGLE_PLAYER) {
            
            bool result =  d1.initDriverProcessing(std::string([hex UTF8String]));
            if (result) {

                std::vector<std::string> players;
                players.push_back(d1.responsePackager.m_player);

                //responseCount++;
                FMResponse = ResponsePackager::packageFMresponse(players[0]);
                FMLOG(VERBOSE, "packageFMResponse", FMResponse.c_str());
                d1.responsePackager.resestVariables();
            }
        }
        else{
            processForMP(std::string([hex UTF8String]));
        }
        
    } else {
        NSLog(@"bleDidReceiveData: empty data");
    }
}


void processForMP(std::string _data) {
    //TODO : _Multiplayer
    bool result = d1.initDriverProcessing(_data);
    bool result1 = d2->initDriverProcessing(_data);

    //FMLOG(VERBOSE, "packageFMResponse", (std::to_string(result) + " " + std::to_string(result1)).c_str());

    if(!result){
        if(d1.responsePackager.m_player == ""){
            d1.responsePackager.m_actionId = ActionIdentifierTable::NULL_ID;
            d1.responsePackager.setPlayerData(1);
        }
    }
    if(!result1){
        if(d2->responsePackager.m_player == ""){
            d2->responsePackager.m_actionId = ActionIdentifierTable::NULL_ID;
            d2->responsePackager.setPlayerData(2);
        }

    }
    if(result || result1){

        //FMLOG(VERBOSE, "packageFMResponse", (d1.responsePackager.m_player + " -- data -- " + d2->responsePackager.m_player).c_str());
        std::vector<std::string> players;

        d1.responsePackager.m_player = d1.responsePackager.m_player.substr(0,7) +
                ", \"count\":" + std::to_string(d1.responseCount) +
                d1.responsePackager.m_player.substr(7);

        d2->responsePackager.m_player = d2->responsePackager.m_player.substr(0,7) +
                ", \"count\":" + std::to_string(d2->responseCount) +
                d2->responsePackager.m_player.substr(7);

        players.push_back(d1.responsePackager.m_player);
        d1.responsePackager.resestVariables();

        players.push_back(d2->responsePackager.m_player);
        d2->responsePackager.resestVariables();

        //FMLOG(VERBOSE, "packageFMResponse", (players[0] + " " + players[1]).c_str());

        std::string _playersData = players[0] + "," + players[1];
        FMResponse = ResponsePackager::packageFMresponse(_playersData);
        FMLOG(VERBOSE, "packageFMResponse", FMResponse.c_str());


    }

}


-(void) bleDidChangeState: (CBManagerState) state {
    NSLog(@"state of ble: %ld", (long)state);
    if(state == 5)
        [BLEFrameworkDelegate SendUnityMessage:BLEUnityMessageName_OnBleDidInitialize message:@"Success"];
}
#pragma mark Instance Methodss
/*
 -(void) readRSSITimer:(NSTimer *)timer
 {
 [ble readRSSI];
 }
 */


- (void)scanForPeripherals
{
    self.searchDevicesDidFinish = false;
    
    if (self.ble.peripherals)
    {
        self.ble.peripherals = nil;
    }
    
    [self.ble findBLEPeripherals:2];
    
    [NSTimer scheduledTimerWithTimeInterval:(float)2.0 target:self selector:@selector(connectionTimer:) userInfo:nil repeats:NO];
}

-(void) connectionTimer:(NSTimer *)timer
{
    if (self.ble.peripherals.count > 0)
    {
        [self.mDevices removeAllObjects];
        [self.mDevicesObject removeAllObjects];
        
        int i;
         for (i = 0; i < self.ble.peripherals.count; i++)
        {
            CBPeripheral *p = [self.ble.peripherals objectAtIndex:i];
            [self.mDevices insertObject:p.identifier.UUIDString atIndex:i];
            [self.mDevicesObject insertObject:[NSString stringWithFormat: @"%@|%@", p.identifier.UUIDString, p.name] atIndex:i];
            
        }
        
        [BLEFrameworkDelegate SendUnityMessage:BLEUnityMessageName_OnBleDidCompletePeripheralScan message:@"Success"];

    }
    else
    {
        NSLog(@"No peripherals found");
        [self.mDevices removeAllObjects];
        [BLEFrameworkDelegate SendUnityMessage:BLEUnityMessageName_OnBleDidCompletePeripheralScan message:@"Fail: No device found"];
    }
    
    
    self.searchDevicesDidFinish = true;
}

- (bool)connectPeripheral:(NSString *)peripheralID
{
    
    CBPeripheral *selectedPeripheral;
    for (CBPeripheral *p in self.ble.peripherals)
    {
        if ([p.identifier.UUIDString isEqualToString:peripheralID])
        {
            selectedPeripheral = p;
        }
    }
    
    if (selectedPeripheral != nil)
    {
        [self.ble connectPeripheral:selectedPeripheral];
        return true;
    }
    
    return false;
}

- (bool)connectPeripheralAtIndex:(NSInteger)index
{
    if (index >= self.ble.peripherals.count)
    {
        return false;
    }
    else if ([self.ble.peripherals objectAtIndex:index]!=nil)
    {
        [self.ble connectPeripheral:[self.ble.peripherals objectAtIndex:index]];
        return true;
    }
    
    return false;
}

- (void)sendDataToPeripheral:(UInt8 *)buf length:(NSUInteger) length
{
    //UInt8 buf[3] = {0x01, 0x00, 0x00};
    NSData *data = [[NSData alloc] initWithBytes:buf length:length];
    [self.ble write:data];
}

- (void)disconnect {
    [self.ble disconnect];
}


#pragma mark Class Methods

+(void)SendUnityMessage:(NSString*)functionName arrayValuesToPass:(NSArray*)arrayValues
{
#ifdef UNITY_VERSION
    NSError *error;
    NSDictionary *jsonObjectToSerialize = [NSDictionary dictionaryWithObject:arrayValues forKey:@"data"];
    NSData *dictionaryData = [NSJSONSerialization dataWithJSONObject:jsonObjectToSerialize options:NSJSONWritingPrettyPrinted error:&error];
    NSString* jsonArrayValues = [NSString stringWithCString:(const char *)[dictionaryData bytes] encoding:NSUTF8StringEncoding];
    UnitySendMessage(strdup([@"BLEControllerEventHandler" UTF8String]), strdup([functionName UTF8String]), strdup([jsonArrayValues UTF8String]));
#endif
}

+(void)SendUnityMessage:(NSString*)functionName message:(NSString*)message
{
#ifdef UNITY_VERSION
    UnitySendMessage(strdup([@"BLEControllerEventHandler" UTF8String]), strdup([functionName UTF8String]), strdup([message UTF8String]));
#endif
}
@end


static BLEFrameworkDelegate* delegateObject = nil;
static NSString* lastConnectedUUID = @"";

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}



// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules

extern "C" {

    void _InitBLEFramework ()
    {
        if(delegateObject == nil){
            delegateObject = [[BLEFrameworkDelegate alloc] init];
        }
        else if( ! [lastConnectedUUID isEqualToString:@""] ){
             //TODO reconnect codebase
             [delegateObject connectPeripheral:lastConnectedUUID];
        }else{
            [delegateObject scanForPeripherals];
        }
    }
    
    void _ScanForPeripherals ()
    {
        [delegateObject scanForPeripherals];
    }
    
    bool _IsDeviceConnected()
    {
        return [delegateObject isConnected];
    }
    
    const char* _GetListOfDevices ()
    {
       if ([delegateObject searchDevicesDidFinish]/* && [delegateObject mDevices].count > 0*/)
        {
            NSMutableString * result = [[NSMutableString alloc] init];
            
            
            for (int i=0; i < [[delegateObject mDevicesObject] count]; i++) {
                if( i != [[delegateObject mDevicesObject] count]-1)
                    [result appendString:[NSString stringWithFormat: @"%@%@", [[delegateObject mDevicesObject][i] description], @","]];
                else
                    [result appendString:[NSString stringWithFormat: @"%@%@", [[delegateObject mDevicesObject][i] description], @""]];
            }
            
            NSString *immutableString = [NSString stringWithString:result];

            return MakeStringCopy([immutableString UTF8String]);
        }
        
        return NULL;
    }
    
    void _Disconnect() {
        [delegateObject disconnect];
    }
    
    bool _ConnectPeripheral(unsigned char *peripheralID)
    {
        bool result = [delegateObject connectPeripheral:[NSString stringWithFormat:@"%s" , peripheralID]];
        if(result)
            lastConnectedUUID = [NSString stringWithFormat:@"%s" , peripheralID];
        return result;
    }
    
    bool _ConnectPeripheralAtIndex(int device)
    {
        return [delegateObject connectPeripheralAtIndex:(NSInteger)device];
    }
    
    void _SendData (unsigned char *buffer, int length)
    {
        [delegateObject sendDataToPeripheral:(UInt8 *)buffer length: length];
    }
    
    int _GetData(unsigned char **data, int size)
    {
        if (delegateObject.dataRx != nil) {
            memcpy(data, [delegateObject.dataRx bytes], 127);
            NSLog(@"The data saved is %s", *data);
            return 0;
        } else {
            NSLog(@"something is wrong. dataRx is nil");
            return -1;
        }
    }


    // Driver realted API'S
    const char* _getFMResponse(){
        return MakeStringCopy(FMResponse.c_str());
    }

    void _setGameMode(int _gameMode){
        
        if(DriverControl::gameMode == SINGLE_PLAYER && _gameMode == MULTI_PLAYER){
               d2.reset(new DriverControl(2));
       }
       else if(DriverControl::gameMode == MULTI_PLAYER && _gameMode == SINGLE_PLAYER){
           DriverControl::playerCounter--;
       }

       DriverControl::gameMode = _gameMode;
        
    }

    int _getGameMode(){
        return DriverControl::gameMode;
    }

    void _setGameID(int _clusterID){
        d1.setClusterID(_clusterID);
    }

    void _setGameID_Multiplayer(int _P1_gameID, int _P2_gameID){
        d1.setClusterID(_P1_gameID);
        d2->setClusterID(_P2_gameID);
    }

    int _getGameID(){
        return d1.getClusterID();
    }

    int _getGameID_Multiplayer(int _playerID){
        if(_playerID == 1)
            return d1.getClusterID();
        else
            return d2->getClusterID();
    }

    const char*  _getDriverVersion(){
        return MakeStringCopy(DRIVER_VERSION);
    }


}
