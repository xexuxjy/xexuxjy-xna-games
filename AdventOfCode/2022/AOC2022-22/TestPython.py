from time import time
from math import ceil, prod
import numpy as np
import re

ALL_BOTS = np.array([np.array([1, 0, 0, 0], dtype=int),
                     np.array([0, 1, 0, 0], dtype=int),
                     np.array([0, 0, 1, 0], dtype=int),
                     np.array([0, 0, 0, 1], dtype=int)])


def timer_func(func):
    # This function shows the execution time of
    # the function object passed
    def wrap_func(*args, **kwargs):
        t1 = time()
        result = func(*args, **kwargs)
        t2 = time()
        print(f'Function {func.__name__!r} executed in {(t2 - t1):.4f}s')
        return result

    return wrap_func


@timer_func
def day19(filepath, part2=False):
    with open(filepath) as fin:
        lines = fin.readlines()

    blueprints = {}
    for line in lines:
        r = re.findall(r'(\d+)', line)
        cost_matrix = np.zeros((4, 4))
        cost_matrix[0, 0] = int(r[1])
        cost_matrix[0, 1] = int(r[2])
        cost_matrix[0, 2] = int(r[3])
        cost_matrix[1, 2] = int(r[4])
        cost_matrix[0, 3] = int(r[5])
        cost_matrix[2, 3] = int(r[6])
        blueprints[int(r[0])] = cost_matrix

    max_geodes = []
    for bp_id, cost_matrix in blueprints.items():
        current_max = 0
        if part2 and bp_id == 4:
            break
        resources = np.array([0, 0, 0, 0], dtype=int)
        bots = np.array([1, 0, 0, 0], dtype=int)
        if not part2:
            initial_state = (24, resources, bots, None)
        else:
            initial_state = (32, resources, bots, None)
        states = set()
        q = [initial_state]
        while q:
            time_left, resources, bots, next_bot = q.pop()
            if next_bot is not None:
                cost_for_next_bot = cost_matrix @ next_bot
                res_for_next_bot = cost_for_next_bot - resources
                time_for_next_bot = int(
                    max([ceil(c / d) if (c > 0 and d > 0) else 0 for c, d in zip(res_for_next_bot, bots)])) + 1
                if time_left - time_for_next_bot <= 0:
                    resources = resources + bots * time_left
                    if resources[-1] > 0:
                        states.add((time_left, tuple(resources.tolist()), tuple(bots.tolist())))
                        current_max = max(current_max, resources[-1])
                    continue
                resources = resources + time_for_next_bot * bots - cost_for_next_bot
                time_left = time_left - time_for_next_bot
                bots = bots + next_bot
            if (time_left > 0 and
               (resources[-1] + bots[-1] * time_left + (time_left - 1) * (time_left) // 2) > current_max):
                available_bots = ALL_BOTS[:((bots > 0).sum() + 1 if (bots > 0).sum() < 4 else 4)]
                rows_to_remove = []
                if bots[0] >= cost_matrix[0].max():
                    rows_to_remove.append(0)
                if bots[1] >= cost_matrix[1, 2]:
                    rows_to_remove.append(1)
                if bots[2] >= cost_matrix[2, 3]:
                    rows_to_remove.append(2)
                available_bots = np.delete(available_bots, rows_to_remove, 0)
                for bot in available_bots:
                    q.append((time_left, resources, bots, bot))
        max_geode = int(max([res[3] for _, res, _ in states] + [0]))
        if part2:
            max_geodes.append(max_geode)
        else:
            max_geodes.append(bp_id * max_geode)
    if not part2:
        return sum(max_geodes)
    else:
        return prod(max_geodes)


def main():
    assert day19('test19') == 33
    print(f"Part 1: {day19('input19')}")

    assert day19('test19', True) == 56 * 62
    # print(f"Part 2 test: {day19('test19', True)}")
    print(f"Part 2: {day19('input19', True)}")


if __name__ == '__main__':
    main()
