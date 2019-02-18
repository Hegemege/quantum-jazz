import gym
import gym_stirap
import numpy as np

import time 

env = gym.make('stirap-v0')

reward = np.zeros(env.timesteps)
env.reset()

tic = time.process_time()
for t in range(env.timesteps):

    # Perform a random action
    action = env.action_space.sample()
    observation, reward[t], done, info = env.step(action)

    if done:
        print("Episode finished after {} timesteps".format(t+1))
        break

toc = time.process_time()
print("Score: ", np.sum(reward))
print("Elapsed time: {} s".format(toc - tic))