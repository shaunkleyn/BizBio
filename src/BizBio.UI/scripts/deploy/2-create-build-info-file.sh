    set -e                                                                                                                                
    BUILD_NUM="$(Build.BuildNumber)"                                                                                                      
    RELEASE_NAME="$(Release.ReleaseName)"                                                                                                 
    ATTEMPT="$(Release.AttemptNumber)"                                                                                                    
    VERSION="${BUILD_NUM}_${RELEASE_NAME}_${ATTEMPT}"                                                                                     
                                                                                                                                          
    printf '{"build":"%s","release":"%s","attempt":"%s","version":"%s"}\n' \                                                              
      "$BUILD_NUM" "$RELEASE_NAME" "$ATTEMPT" "$VERSION" \                                                                                
      > /var/www/snaptap/ui/current/public/build-info.json  