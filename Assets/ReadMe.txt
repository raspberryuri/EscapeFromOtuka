�yUnity + Git ������Ǝ菇���z

���̎菇���́AUnity�v���W�F�N�g��Git�ŊǗ����A�����l�ŋ����J�����邽�߂̊�{�I�ȍ�Ǝ菇���܂Ƃ߂Ă��܂��B

---

�� 1. ��������

�EUnity�̃o�[�W�������`�[���œ��ꂵ�܂��傤�B
�EGit�̃A�J�E���g��p�ӂ��Ă��������B
�E���|�W�g��URL�����L���A�N���[���i���[�J���ɃR�s�[�j���Ă��������B

  git clone https://github.com/���[�U�[��/���|�W�g����.git

---

�� 2. ��ƑO�̏���

�E��ƊJ�n�O�ɕK���ŐV�̃����[�g���|�W�g�����擾���܂��B

  git pull origin main

---

�� 3. �V������ƃu�����`���쐬����i�����j

�E����main�u�����`�ō�Ƃ����A�V�����u�����`�����܂��B

  git checkout -b feature/�u�����`��

---

�� 4. Unity�ō�Ƃ��s��

�E�V�����X�N���v�g��V�[���Ȃǂ��쐬�E�ҏW���܂��B
�EUnity�̎��������t�@�C���iLibrary�t�H���_�Ȃǁj��Git�Ǘ����Ȃ��悤.gitignore�ŏ��O����Ă��܂��B

---

�� 5. �ύX��Git�ɔ��f����

�E�ύX���e���m�F����

  git status

�E�ύX�t�@�C�����X�e�[�W�ɓo�^����

  git add .

�E�R�~�b�g���b�Z�[�W�����ăR�~�b�g����

  git commit -m "��Ɠ��e�̐���"

---

�� 6. ��ƃu�����`�������[�g��push����

  git push origin �u�����`��

---

�� 7. �v�����N�G�X�g�iPull Request�j���쐬����

�EGitHub�Ȃǂ̃��|�W�g���Ǘ���ʂŁA��ƃu�����`����main�u�����`�ւ̃v�����N�G�X�g���쐬���܂��B
�E�R�[�h���r���[�⓮��m�F���s���A���Ȃ����main�u�����`�Ƀ}�[�W���܂��B

---

�� 8. ��ƏI�����main�u�����`�̍ŐV��

�Emain�u�����`�ɐ؂�ւ��čŐV���擾���܂��B

  git checkout main
  git pull origin main

---

�� ���ӎ���

�Emain�u�����`�ɒ���push���Ȃ��ł��������B�K���v�����N�G�X�g���o�ă}�[�W���܂��傤�B
�E�R���t���N�g�i�����j���N������A�����o�[���m�ő��k���ĉ������Ă��������B
�E.gitignore�͕K���ݒ肵�āA�s�v�t�@�C�������L���Ȃ��悤�ɂ��܂��傤�B
�E�R�~�b�g���b�Z�[�W�͒N�����Ă��킩��₷���Ȍ��ɏ����܂��傤�B

---

�� Git���[�U�[���̐ݒ�i����̂݁j

  git config --global user.name "���Ȃ��̖��O"
  git config --global user.email "���Ȃ��̃��[���A�h���X"

---

�� Git�R�}���h�̊ȒP����

�Egit clone �F���|�W�g�����R�s�[����
�Egit pull �F�����[�g�̍ŐV���擾���ă}�[�W����
�Egit add �F�ύX�t�@�C����o�^����
�Egit commit �F�ύX�����[�J���ɋL�^����
�Egit push �F���[�J���̕ύX�������[�g�ɑ���
�Egit checkout �F�u�����`��؂�ւ���
�Egit branch �F�u�����`�ꗗ��\������

---

���̎菇������āA�~���ȋ����J����ڎw���܂��傤�I

