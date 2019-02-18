from gym_stirap import StirapEnv
import numpy as np
import warnings
import pytest

@pytest.mark.parametrize("final_reward",[True, False])
def test_make(final_reward):
    env = StirapEnv(final_reward)
    assert env is not None

    observation = env.reset()
    print(observation)
    assert isinstance(observation, np.ndarray)
    assert env.observation_space.contains(observation)

    action = env.action_space.sample()
    assert isinstance(action, np.ndarray)
    observation_2, reward, done, psi = env.step(action)

    assert isinstance(psi, np.ndarray)
    assert psi.shape == (env.n,)

    assert env.observation_space.contains(observation_2)
    assert isinstance(reward, float)
    assert done == False
    