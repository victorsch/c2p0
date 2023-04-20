
# C2P0

```
//                                                                                  
//                                                                                  
//          CCCCCCCCCCCCC 222222222222222    PPPPPPPPPPPPPPPPP        000000000     
//       CCC::::::::::::C2:::::::::::::::22  P::::::::::::::::P     00:::::::::00   
//     CC:::::::::::::::C2::::::222222:::::2 P::::::PPPPPP:::::P  00:::::::::::::00 
//    C:::::CCCCCCCC::::C2222222     2:::::2 PP:::::P     P:::::P0:::::::000:::::::0
//   C:::::C       CCCCCC            2:::::2   P::::P     P:::::P0::::::0   0::::::0
//  C:::::C                          2:::::2   P::::P     P:::::P0:::::0     0:::::0
//  C:::::C                       2222::::2    P::::PPPPPP:::::P 0:::::0     0:::::0
//  C:::::C                  22222::::::22     P:::::::::::::PP  0:::::0 000 0:::::0
//  C:::::C                22::::::::222       P::::PPPPPPPPP    0:::::0 000 0:::::0
//  C:::::C               2:::::22222          P::::P            0:::::0     0:::::0
//  C:::::C              2:::::2               P::::P            0:::::0     0:::::0
//   C:::::C       CCCCCC2:::::2               P::::P            0::::::0   0::::::0
//    C:::::CCCCCCCC::::C2:::::2       222222PP::::::PP          0:::::::000:::::::0
//     CC:::::::::::::::C2::::::2222222:::::2P::::::::P           00:::::::::::::00 
//       CCC::::::::::::C2::::::::::::::::::2P::::::::P             00:::::::::00   
//          CCCCCCCCCCCCC22222222222222222222PPPPPPPPPP               000000000     
//                                                                                  
//                                                                                  
//                                                                                  
//                                                                                  
//                                                                                  
//                                                                                  
//    
```

Welcome to my crack at a C2 framework. This framework is meant to be modular with thorough documentation on creating your own beacons and customizing for your red teams needs. The framework is designed to put the control in the hands of the "command" part of the framework. Agents communicate to listening posts which communicate back to the operator's server (or web GUI). Though this first commit is very barebones, the plan is to have later communication occur entirely over encrypted channels that can be predefined at a beacon's launch. The listening posts will be prepared to accept connections from agents where the agent will identify itself, and its preferred means of exfiltration in order to let the listening post know what channel to expect communication from this agent. Listening posts communicate back to the host via HTTPS, but the plan is to have agents communicate to listening post over a variety of means only limited by the creativity of whoever is forking this repo.

My initial version will communicate solely over TCP sockets, but in the next commit I plan to add further layers of abstraction where listening posts can be changed to listen via different means than sockets, and accept connections over different means as well. While there will be a few example beacons in this repo, those will mainly be the responsibility of the user to customize or create anew.