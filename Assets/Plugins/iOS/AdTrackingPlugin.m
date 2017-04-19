//
//  AdTrackingPlugin.m
//  Unity-iPhone
//
//  Created by 上海都玩 on 2016/12/26.
//
//

#import "AdTrackingPlugin.h"
#import "TalkingDataAppCpa.h"

@implementation AdTrackingPlugin 
#ifdef __cplusplus
extern "C" {
#endif
    void iOS_InitAdTracking ()
    {
        [TalkingDataAppCpa init:@"83705FE46CC046088D9E8E52FA6A494F" withChannelId:@"AppStore"];
		NSLog(@"[ADD TRACKING][INIT]");
    }
    
    void iOS_AdTracking_OnRegister (char* accountID)
    {
        [TalkingDataAppCpa onRegister:[NSString stringWithUTF8String:accountID]];
        NSLog(@"[ADD TRACKING][ON REGISTER]");
    }
    
    void iOS_AdTracking_OnLogin(char* accountID)
    {
        [TalkingDataAppCpa onLogin:[NSString stringWithUTF8String:accountID]];
        NSLog(@"[ON LOGIN]");
    }
    
    void iOS_AdTracking_OnPay(char* accountID, char* orderID, int amount, char* currencyType, char* payType)
    {
        [TalkingDataAppCpa onPay:[NSString stringWithUTF8String:accountID] withOrderId:[NSString stringWithUTF8String:orderID] withAmount:600 withCurrencyType:[NSString stringWithUTF8String:currencyType] withPayType:[NSString stringWithUTF8String:payType]];
        NSLog(@"[ON PAY]");
    }
#ifdef __cplusplus
}
#endif
@end
