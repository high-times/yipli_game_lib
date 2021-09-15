#import <Foundation/Foundation.h>


extern NSString *const BLEUnityMessageName_OnBleDidInitialize;
extern NSString *const BLEUnityMessageName_OnBleDidCompletePeripheralScan;
extern NSString *const BLEUnityMessageName_OnBleDidConnect;
extern NSString *const BLEUnityMessageName_OnBleDidDisconnect;
extern NSString *const BLEUnityMessageName_OnBleDidReceiveData;

@interface BLEFrameworkDelegate : NSObject
-(void) scanForPeripherals;
-(bool) connectPeripheral:(NSString *)peripheralID;
@property (readonly, strong, nonatomic) NSData *dataRx;


@end

@interface AppDelegate : NSObject
@end
