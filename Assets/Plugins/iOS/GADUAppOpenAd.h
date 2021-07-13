// Copyright 2021 Google LLC. All Rights Reserved.

#import <Foundation/Foundation.h>

#import <GoogleMobileAds/GoogleMobileAds.h>

#import "GADUTypes.h"

@interface GADUAppOpenAd : NSObject

/// Initializes a GADUAppOpenAd.
- (instancetype)initWithAppOpenAdClientReference:(GADUTypeAppOpenAdClientRef *)appOpenAdClient;

/// The app open ad.
@property(nonatomic, strong) GADAppOpenAd *appOpenAd;

/// A reference to the Unity app open ad client.
@property(nonatomic, assign) GADUTypeAppOpenAdClientRef *appOpenAdClient;

/// The ad loaded callback into Unity.
@property(nonatomic, assign) GADUAppOpenAdLoadedCallback adLoadedCallback;

/// The ad request failed callback into Unity.
@property(nonatomic, assign) GADUAppOpenAdFailedToLoadCallback adFailedToLoadCallback;

/// The paid event callback into Unity.
@property(nonatomic, assign) GADUAppOpenAdPaidEventCallback paidEventCallback;

/// The ad failed to present full screen content callback into Unity.
@property(nonatomic, assign) GADUAppOpenAdFailedToPresentFullScreenContentCallback
    adFailedToPresentFullScreenContentCallback;

/// The ad presented full screen content callback into Unity.
@property(nonatomic, assign)
    GADUAppOpenAdDidPresentFullScreenContentCallback adDidPresentFullScreenContentCallback;

/// The ad dismissed full screen content callback into Unity.
@property(nonatomic, assign)
    GADUAppOpenAdDidDismissFullScreenContentCallback adDidDismissFullScreenContentCallback;

/// The ad impression callback into Unity.
@property(nonatomic, assign) GADUAppOpenAdDidRecordImpressionCallback adDidRecordImpressionCallback;

// The app open ad response info.
@property(nonatomic, readonly, copy) GADResponseInfo *responseInfo;

/// Makes an ad request. Additional targeting options can be supplied with a request object.
- (void)loadWithAdUnitID:(NSString *)adUnit
             orientation:(GADUScreenOrientation)orientation
                 request:(GADRequest *)request;

/// Shows the app open ad.
- (void)show;

@end
