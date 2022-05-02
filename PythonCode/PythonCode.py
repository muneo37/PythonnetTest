import tensorflow as tf

# �悭���肪���Ȃ��傤��Ȃ��e�X�g�R�[�h
class Calculator:
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def add(self):
        self.y = self.x + self.y
        return self.y

    @staticmethod
    def sum(num_list):
        return sum(num_list)

def GetDict():
    return {
        0 : "this",
        1 : "is",
        2 : "my",
        3 : "dictionary",
        }

def tensor():
     hello = tf.constant('Hello, TensorFlow!')
     sess = tf.Session()
     return sess.run(hello)
