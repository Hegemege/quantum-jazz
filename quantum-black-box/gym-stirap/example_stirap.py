from gym_stirap import StirapEnv
import numpy as np

env = StirapEnv(initial_displacement=-.0)
obs = []
observation = env.reset()
obs.append(observation)

actions_left = [-.75] * 400
actions_right = [0.75] * 400

actions = np.array([actions_left, actions_right]).T

#reward = np.zeros(env.timesteps)
rewards = []
t = 0
while True:
    env.render()

    try:
        action = actions[t]
    except:
        action = np.array([0.,0.])

    observation, reward, done, info = env.step(action)
    t += 1
    rewards.append(reward)
    obs.append(observation)
    if done:
        print("Episode finished after {} timesteps".format(t+1))
        break

print("Score: ", np.sum(rewards))
print(obs[-1])