  set -e                                                                                                                                  
                                                                                                                                          
                    TARGET_DIR="$(TargetDirectory)"                                                                                       
                    RELEASE_ID="$(Build.BuildId)"                                                                                         
                    RELEASE_DIR="$TARGET_DIR/releases/$RELEASE_ID"                                                                        
                                                                                                                                          
                                                                                                                                          
                    #echo "=========================================="                                                                    
                    #echo "Reloading PM2"                                                                                                 
                    #echo "=========================================="                                                                    
                    # Reload PM2                                                                                                          
                    cd $TARGET_DIR                                                                                                        
                    if pm2 list | grep -q "snaptap-frontend"; then                                                                        
                      pm2 reload ecosystem.config.cjs --only snaptap-frontend --env production                                            
                    else                                                                                                                  
                      pm2 start ecosystem.config.cjs --only snaptap-frontend --env production                                             
                    fi                                                                                                                    
                    pm2 save                                                                                                              
                                                                                                                                          
                                                                                                                                          
                    # Cleanup old releases (keep last 5)                                                                                  
                    #echo "=========================================="                                                                    
                    #echo "Removing redundant releases"                                                                                   
                    #echo "=========================================="                                                                    
                    cd $TARGET_DIR/releases                                                                                               
                    ls -dt */ | tail -n +6 | xargs -r rm -rf                                                                              
                                                                                                                                          
                                                                                                                                          
                    echo "=========================================="                                                                     
                    echo "Dev Deployment Complete!"                                                                                       
                    echo "==========================================" 