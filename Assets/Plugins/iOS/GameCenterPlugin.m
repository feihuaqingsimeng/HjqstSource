//
//  GameCenterPlugin.m
//  Unity-iPhone
//
//  Created by 上海都玩 on 2017/2/24.
//
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import "GameCenterPlugin.h"
#import "UnityAppController.h"

@implementation GameCenterPlugin
#ifdef __cplusplus
extern "C" {
#endif
    void authenticateLocalUser()
    {
        __weak GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        [localPlayer setAuthenticateHandler:^(UIViewController * viewController, NSError * error)
        {
            if (viewController != nil)
            {
                [GetAppController().rootViewController presentViewController:viewController animated:true completion:nil];
            }
            else if (localPlayer.isAuthenticated)
            {
            }
            else
            {
            }
        }];
    }
    
    void authenticateComplete (bool authenticatSuccess, const char *playerID)
    {
    }
#ifdef __cplusplus
}
#endif
@end
